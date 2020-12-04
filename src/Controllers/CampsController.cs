using System;
using System.Collections.Generic;
using System.Data.SqlTypes;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CoreCodeCamp.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CampsController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;

        public CampsController(ICampRepository campRepository, IMapper mapper)
        {
            _campRepository = campRepository;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<CampModel[]>> GetAllCamps(bool includeTalks = false)
        {
            try
            {
                var results = await _campRepository.GetAllCampsAsync(includeTalks);

                if (results == null) NotFound("Camps not found");

                return Ok(_mapper.Map<CampModel[]>(results));
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }
           
        }

        [HttpGet("{moniker}", Name= nameof(GetCampsByName))]
        public async Task<ActionResult<CampModel>> GetCampsByName(string moniker)
        {
            try
            {
                var result = await _campRepository.GetCampAsync(moniker, false);

                if (result == null) return NotFound($"{moniker} not found");

                return _mapper.Map<CampModel>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }
        }

        [HttpGet("search")]
        public async Task<ActionResult<CampModel[]>> SearchByDate(DateTime date, bool includeTalks = false)
        {
            try
            {
                var result = await _campRepository.GetAllCampsByEventDate(date, includeTalks);

                if (!result.Any()) return NotFound("Data not found");

                return _mapper.Map<CampModel[]>(result);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }
        }

        [HttpPost]
        public async Task<ActionResult<CampModel>> CreateCamp(CampModel model)
        {
            try
            {
                //if(ModelState.IsValid){}  OR  [ApiController] at Controller Definition
                var campExist = await _campRepository.GetCampAsync(model.Moniker);
                if (campExist != null)
                {
                    return BadRequest($"{model.Moniker} is already exists");
                }

                var camp = _mapper.Map<Camp>(model);
                _campRepository.Add(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return CreatedAtRoute(nameof(GetCampsByName), new { campName = model.Moniker}, _mapper.Map<CampModel>(camp));
                }
               
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }

            return BadRequest();
        }

        [HttpPut("{moniker}")]
        public async Task<ActionResult<CampModel>> UpdateCamp(string moniker, CampModel model)
        {
            try
            {
                //if(ModelState.IsValid){}  OR  [ApiController] at Controller Definition
                var campExist = await _campRepository.GetCampAsync(model.Moniker);
                if (campExist == null)
                {
                    return BadRequest($"{model.Moniker} not found");
                }

                _mapper.Map(model, campExist);

                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<CampModel>(campExist);
                }

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }

            return BadRequest();
        }

        [HttpDelete("{moniker}")]
        public async Task<IActionResult> DeleteCamp(string moniker)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);
                if (camp == null) return NotFound($"{moniker} not found");

                _campRepository.Delete(camp);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }

            return BadRequest();
        }
    }
}