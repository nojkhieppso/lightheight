﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MvcCoreApiWithAuthentication.Models;

namespace MvcCoreApiWithAuthentication.Controllers
{
    [Route("[controller]")]
    public class ContactsController : ControllerBase
    {
        private readonly IContactRepository _repository;

        public ContactsController(IContactRepository repository)
        {
            _repository = repository;
        }

        [HttpGet("")]
        public async Task<IActionResult> Get()
        {
            return Ok(await _repository.GetAll());
        }

        [HttpGet("{id}", Name = "GetContactById")]
        public async Task<ActionResult> Get(int id)
        {
            var contact = await _repository.Get(id);
            if (contact == null)
            {
                return NotFound();
            }

            return Ok(contact);
        }

        [HttpPost("")]
        public async Task<IActionResult> Post([FromBody]Contact contact)
        {
            if (ModelState.IsValid)
            {
                var newId = await _repository.Add(contact);
                return CreatedAtRoute("GetContactById", new { id = newId }, contact);
            }

            return BadRequest(ModelState);
        }

        [HttpPut("{id}")]
        [Authorize("WritePolicy")]
        public async Task<IActionResult> Put(int id, [FromBody]Contact contact)
        {
            contact.ContactId = id;
            await _repository.Update(contact);
            return NoContent();
        }

        [HttpDelete("{id}")]
        [Authorize("WritePolicy")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _repository.Get(id);
            if (deleted == null)
            {
                return NotFound();
            }

            await _repository.Delete(id);
            return NoContent();
        }
    }
}
