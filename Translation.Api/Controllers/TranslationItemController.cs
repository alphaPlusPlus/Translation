﻿using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Translation.Api.Models;
using Translation.Data;

namespace Translation.Api.Controllers
{
    [Route("api/{language:language}/translationitems")]
    public class TranslationItemController : Controller
    {
        private readonly SqlLiteDbContext _context;

        public TranslationItemController(SqlLiteDbContext context)
        {
            this._context = context;
        }

        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet]
        public ActionResult<TranslationItemDto> GetTranslationItems(string language)
        {
            var translationItems = _context.TranslationItems
                    .Where(m => m.Language.ToLower() == language.ToLower())
                    .ToList();

            return Ok(translationItems.Select(TranslationItemDto.Construct));
        }

        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [HttpGet("{key}")]
        public ActionResult<TranslationItemDto> GetTranslationItem(string language, string key)
        {
            var translationItem = _context.TranslationItems
                .FirstOrDefault(m => m.Key == key && m.Language == language);

            if (translationItem == null)
            {
                return NotFound();
            }

            return Ok(TranslationItemDto.Construct(translationItem));
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<TranslationItemDto> CreateTranslationItem([FromBody]TranslationItemDto dto, string language)
        {
            var duplicateTranslationItemExist = _context.TranslationItems
                .Any(x => x.Key == dto.Key);

            if (duplicateTranslationItemExist)
            {
                ModelState.AddModelError(nameof(TranslationItemDto.Key), $"{nameof(TranslationItemDto.Key)} is not unique");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var translationItem = TranslationItemDto.Deconstruct(dto);
            translationItem.Language = language;


            _context.TranslationItems.Add(translationItem);
            _context.SaveChanges();

            dto = TranslationItemDto.Construct(translationItem);

            return Created($"api/{language}/translationItems/{translationItem.Key}", dto);
        }


        [HttpDelete("{key}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult DeleteTranslationItem(string language, string key)
        {
            var translationItem = _context.TranslationItems
                .FirstOrDefault(m => m.Key == key && m.Language == language);

            if (translationItem == null)
            {
                return NotFound();
            }

            _context.TranslationItems.Remove(translationItem);
            _context.SaveChanges();

            return Ok();
        }
    }
}