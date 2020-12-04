using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CoreCodeCamp.Data;
using CoreCodeCamp.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Routing;

namespace CoreCodeCamp.Controllers
{
    [Route("api/camps/{moniker}/talks")]
    [ApiController]
    public class TalksController : ControllerBase
    {
        private readonly ICampRepository _campRepository;
        private readonly IMapper _mapper;
        private readonly LinkGenerator _linkGenerator;

        public TalksController(ICampRepository campRepository, IMapper mapper, LinkGenerator linkGenerator)
        {
            _campRepository = campRepository;
            _mapper = mapper;
            _linkGenerator = linkGenerator;
        }

        [HttpGet]
        public async Task<ActionResult<TalkModel[]>> GetTalks(string moniker)
        {
            try
            {
                var results = await _campRepository.GetTalksByMonikerAsync(moniker);

                return _mapper.Map<TalkModel[]>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        [HttpGet("{id}", Name = "GetTalk")]
        public async Task<ActionResult<TalkModel>> GetTalk(string moniker, int id)
        {
            try
            {
                var results = await _campRepository.GetTalkByMonikerAsync(moniker, id);

                if (results == null) return NotFound("Talk not found");

                return _mapper.Map<TalkModel>(results);
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }
        }

        [HttpPost]
        public async Task<ActionResult<TalkModel>> CreateTalk(string moniker, TalkModel model)
        {
            try
            {
                var camp = await _campRepository.GetCampAsync(moniker);

                if (camp == null) return BadRequest("Camp does not exist");

                var talk = _mapper.Map<Talk>(model);
                talk.Camp = camp;

                if(model.Speaker == null) return BadRequest("Speaker ID is required");
                var speaker = await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                if(speaker == null) return BadRequest("Speaker could not found");

                talk.Speaker = speaker;

                _campRepository.Add(talk);

                if (await _campRepository.SaveChangesAsync())
                {
                    var url = _linkGenerator.GetPathByAction(HttpContext, nameof(GetTalk),
                        values: new {moniker = moniker, id = talk.TalkId});
                    return Created(url, _mapper.Map<TalkModel>(talk));
                }

            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

            return BadRequest("Failed to save new Talk");
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<TalkModel>> UpdateTalk(string moniker, int id, TalkModel model)
        {
            try
            {
                var results = await _campRepository.GetTalkByMonikerAsync(moniker, id);
                if (results == null) return BadRequest("Talk not found");

                _mapper.Map(model, results);

                if (model.Speaker != null)
                {
                    var speaker = await _campRepository.GetSpeakerAsync(model.Speaker.SpeakerId);
                    if (speaker != null)
                    {
                        results.Speaker = speaker;
                    }
                }

                if (await _campRepository.SaveChangesAsync())
                {
                    return _mapper.Map<TalkModel>(results);
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Error");
            }

            return BadRequest("Failed to update database");

        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTalk(string moniker, int id)
        {
            try
            {
                var talk = await _campRepository.GetTalkByMonikerAsync(moniker, id);
                if (talk == null) return NotFound("Talk not found");

                _campRepository.Delete(talk);

                if (await _campRepository.SaveChangesAsync())
                {
                    return Ok();
                }
            }
            catch (Exception)
            {
                return this.StatusCode(StatusCodes.Status500InternalServerError, "Database Server Error!");
            }

            return BadRequest("Failed to update database");
        }
    }
}