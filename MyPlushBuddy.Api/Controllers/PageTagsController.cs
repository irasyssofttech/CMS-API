using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using MyPlushBuddy.Api.Helpers;
using MyPlushBuddy.Api.Models;
using MyPlushBuddy.Api.ResourceParameters;
using MyPlushBuddy.Api.Services;
using System;
using System.Collections.Generic;
using System.Text.Json;
using System.Threading.Tasks;

namespace MyPlushBuddy.Api.Controllers
{
    [ApiController]
    [Route("api/pagetags")]
    [Produces("application/json")]
    [Consumes("application/json")]
    public class PageTagsController : ControllerBase
    {
        private readonly ILogger<PageTagsController> logger;
        private readonly IPageTagRepository pageTagRepository;
        private readonly IMapper mapper;
        private readonly IPropertyMappingService propertyMappingService;
        private readonly IUserInfoService userInfoService;

        public PageTagsController(ILogger<PageTagsController> logger,
            IPageTagRepository pageTagRepository,
            IMapper mapper,
            IPropertyMappingService propertyMappingService,
            IUserInfoService userInfoService)
        {
            this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
            this.pageTagRepository = pageTagRepository ?? throw new ArgumentNullException(nameof(pageTagRepository));
            this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            this.propertyMappingService = propertyMappingService ?? throw new ArgumentNullException(nameof(propertyMappingService));
            this.userInfoService = userInfoService ?? throw new ArgumentNullException(nameof(userInfoService));
        }

        /// <summary>
        /// Retun collection of PageTags based on search and filter with pagination header details
        /// </summary>
        /// <param name="pageTagsResourceParam">Parameter for sending \
        /// 'SearchQuery', \
        /// 'Robots' as filter \
        /// PageSize \ 
        /// Page Number</param>
        /// <returns>Collection of PageTags with PageTagId</returns>
        /// <response code="200">Return PageTag detail with PageTagId</response>
        [HttpGet(Name = "GetPageTags")]        
        [ProducesResponseType(StatusCodes.Status200OK)]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<PageTagViewModel>>> GetPageTags(
            [FromQuery] PageTagsResourceParameter pageTagsResourceParam)
        {            
            if (!propertyMappingService.ValidMappingExistsFor<Models.PageTagViewModel, Entities.PageTag>
                (pageTagsResourceParam.OrderBy))
            {
                return BadRequest();
            }

            var pageTagsFromDb = await pageTagRepository.GetPageTagsAsync(pageTagsResourceParam);

            var previousPageLink = pageTagsFromDb.HasPreviour ?
                CreatePageTagsResourceUri(pageTagsResourceParam,
                ResourceUriType.PreviourPage) : null;

            var nextPageLink = pageTagsFromDb.HasNext ?
                CreatePageTagsResourceUri(pageTagsResourceParam,
                ResourceUriType.NextPage) : null;

            var paginationMetaData = new
            {
                totalCount = pageTagsFromDb.TotalCount,
                pageSize = pageTagsFromDb.PageSize,
                currentPage = pageTagsFromDb.CurrentPage,
                totalPages = pageTagsFromDb.TotalPages,
                previousPageLink,
                nextPageLink
            };

            Response.Headers.Add("X-Pagination",
                JsonSerializer.Serialize(paginationMetaData));

            return Ok(mapper.Map<IEnumerable<PageTagViewModel>>(pageTagsFromDb));
        }

        /// <summary>
        /// Provide the PageTag details
        /// </summary>
        /// <param name="pageTagId">PageTagId used to identify the details from database</param>
        /// <returns>Return all pagetag details with pagetagid</returns>
        /// <response code="200">Return PageTag detail with PageTagId</response>                
        [HttpGet("{pageTagId}", Name = "GetPageTag")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]        
        public async Task<ActionResult<PageTagViewModel>> GetPageTag(int pageTagId)
        {            
            var pageTag = await pageTagRepository.GetPageTagAsync(pageTagId);

            if (pageTag == null)
            {
                logger.LogInformation($"PageTag for Id ({pageTagId}) is not found.");
                return NotFound();
            }

            return Ok(mapper.Map<PageTagViewModel>(pageTag));
        }

        /// <summary>
        /// Save new pagetag information posted by client
        /// </summary>
        /// <param name="pageModel"> JSON For saving the new pagetag information </param>
        /// <returns> Return new pagetag created by system </returns>
        /// <response code="201">Return PageTag detail with PageTagId</response>   
        [HttpPost]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status201Created)]
        public async Task<ActionResult<PageTagViewModel>> SavePageTag([FromBody] PageTagCreateModel pageModel)
        {
            var dbPageModel = mapper.Map<Entities.PageTag>(pageModel);

            pageTagRepository.AddPageTag(dbPageModel);

            await pageTagRepository.SaveAsync();

            var pageTagReturn = mapper.Map<Models.PageTagCreateModel>(dbPageModel);

            return CreatedAtAction(
                "GetPageTag",
                new { pageTagId = dbPageModel.PageTagId },
                pageTagReturn);
        }

        /// <summary>
        /// Updated PageTag information for provided pageTagId
        /// </summary>
        /// <param name="pageTagId">PageTagId for which update exeucted</param>
        /// <param name="pageModel">JSON with the properties which client can update</param>
        /// <returns>Successfull return will send 204 NoContent status</returns>
        /// <response code="204">204 NoContent staus will Identify that Update completed successfully</response>  
        [HttpPut("{pageTagId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<IActionResult> UpdatePageTag(int pageTagId, [FromBody] PageTagUpdateModel pageModel)
        {
            var isPageTagExist = await pageTagRepository.PageTagExistAsync(pageTagId);
            if (!isPageTagExist)
            {
                return NotFound();
            }

            var dbPageTag = await pageTagRepository.GetPageTagAsync(pageTagId);

            if (dbPageTag == null)
            {
                return NotFound();
            }

            //Step 1: map the entity to PageTagUpdateModel
            //Step 2: apply the updated field value to that model
            //Step 3: map the PageTagUpdateModel back to an entity
            // AutoMapper will take care for above 3 steps by executing below line.
            mapper.Map(pageModel, dbPageTag);
            await pageTagRepository.SaveAsync();

            return NoContent();
        }

        /// <summary>
        /// Partially update PageTag
        /// </summary>
        /// <param name="pageTagId">The id of the pagetag you want to get</param>
        /// <param name="patchPageTagDocument">The set of operations to apply to the PageTag</param>
        /// <returns>An ActionResult of type PageTag</returns>        
        /// <remarks>Sample request (this request updates the pagetag's **page title**)  \
        /// \
        /// PATCH /pagetags/pageTagId \
        /// [ \
        ///     { \
        ///         "op": "replace", \
        ///         "path": "/pageTitle", \
        ///         "value": "new page title" \
        ///     } \
        /// ] 
        /// </remarks>
        /// <response code="204">204 NoContent staus will Identify that Update completed successfully</response>  
        [HttpPatch("{pageTagId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        public async Task<ActionResult> PartialUpdatePageTag(int pageTagId, [FromBody] JsonPatchDocument<PageTagUpdateModel> patchPageTagDocument)
        {
            var isPageTagExist = await pageTagRepository.PageTagExistAsync(pageTagId);
            if (!isPageTagExist)
            {
                return NotFound();
            }

            var dbPageTag = await pageTagRepository.GetPageTagAsync(pageTagId);

            // Step 1: Convert entity model to manipulation model
            // i.e PageTag Entity (db) to PageTag Creation model
            var pageTagPatch = mapper.Map<PageTagUpdateModel>(dbPageTag);

            // Step 2: Apply the patch document property changes to manipulation model.
            // i.e JsonPatchDocument to Page Tag Creation model.
            patchPageTagDocument.ApplyTo(pageTagPatch, ModelState);

            if (!TryValidateModel(pageTagPatch))
            {
                return ValidationProblem(ModelState);
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (!TryValidateModel(pageTagPatch))
            {
                return BadRequest(ModelState);
            }

            mapper.Map(pageTagPatch, dbPageTag);

            await pageTagRepository.SaveAsync();

            return NoContent();

        }

        private string CreatePageTagsResourceUri(
            PageTagsResourceParameter pageTagsResourceParameter,
            ResourceUriType type)
        {
            switch (type)
            {
                case ResourceUriType.PreviourPage:
                    return Url.Link("GetPageTags",
                        new
                        {
                            orderBy = pageTagsResourceParameter.OrderBy,
                            pageNumber = pageTagsResourceParameter.PageNumber - 1,
                            pageSize = pageTagsResourceParameter.PageSize,
                            robots = pageTagsResourceParameter.Robots,
                            searchQuery = pageTagsResourceParameter.SearchQuery
                        });
                case ResourceUriType.NextPage:
                    return Url.Link("GetPageTags",
                        new
                        {
                            orderBy = pageTagsResourceParameter.OrderBy,
                            pageNumber = pageTagsResourceParameter.PageNumber + 1,
                            pageSize = pageTagsResourceParameter.PageSize,
                            robots = pageTagsResourceParameter.Robots,
                            searchQuery = pageTagsResourceParameter.SearchQuery
                        });
                default:
                    return Url.Link("GetAuthors",
                        new
                        {
                            orderBy = pageTagsResourceParameter.OrderBy,
                            pageNumber = pageTagsResourceParameter.PageNumber,
                            pageSize = pageTagsResourceParameter.PageSize,
                            robots = pageTagsResourceParameter.Robots,
                            searchQuery = pageTagsResourceParameter.SearchQuery
                        });
            }
        }
    }
}
