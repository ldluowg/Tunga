﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using TungaRestaurant.Data;
using TungaRestaurant.Models;

namespace TungaRestaurant.Areas.Manager.Controllers
{
    [Area("Manager")]
    public class TablesController : Controller
    {
        private readonly TungaRestaurantDbContext _context;

        public TablesController(TungaRestaurantDbContext context)
        {
            _context = context;
        }

        // GET: Manager/Tables
        public async Task<IActionResult> Index(int? branch,int? room)
        {
            ViewBag.RoomList = await _context.Rooms.ToListAsync();
            ViewBag.BranchList = await _context.Branch.ToListAsync();
            IQueryable<Table> tunga ;
            ViewBag.Reservations = await _context.Reservations.Where(r => r.CreatedAt >= DateTime.Now.AddMonths(-1)).Include(r=>r.Table).ToListAsync();
            if (branch == null)
            {
                if (room == null)
                {
                    var id = _context.Branch.FirstOrDefault().Id;
                    tunga = from r in _context.Rooms
                            join t in _context.Table
                            on r.Id equals t.RoomId
                            where r.BranchId == id
                            select t;
                    ViewBag.Branch = id;
                    ViewBag.Room = null;
                }
                else
                {
                    var id = _context.Branch.FirstOrDefault().Id;
                    tunga = from r in _context.Rooms
                            join t in _context.Table
                            on r.Id equals t.RoomId
                            where r.BranchId == id
                            where r.Id == room

                            select t;
                    ViewBag.Branch = id;
                    ViewBag.Room = room;
                }
                
            }
            else
            {
                if (room == null)
                {
                    var id = _context.Branch.FirstOrDefault().Id;
                    tunga = from r in _context.Rooms
                            join t in _context.Table
                            on r.Id equals t.RoomId
                            where r.BranchId == branch
                            select t;
                    ViewBag.Branch = branch;
                    ViewBag.Room = null;
                }
                else
                {
                    var id = _context.Branch.FirstOrDefault().Id;
                    tunga = from r in _context.Rooms
                            join t in _context.Table
                            on r.Id equals t.RoomId
                            where r.BranchId == branch
                            where r.Id == room

                            select t;
                    ViewBag.Branch = branch;
                    ViewBag.Room = room;
                }
            }
            


           
                return View(await tunga.ToListAsync());
        }

        // GET: Manager/Tables/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table
                .Include(t => t.Room)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (table == null)
            {
                return NotFound();
            }

            return View(table);
        }

        // GET: Manager/Tables/Create
        public IActionResult Create()
        {
           
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "Id", "Name");
            return View();
        }

        // POST: Manager/Tables/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Type,RoomId,NumberOfGuest,Status")] Table table)
        {
            if (ModelState.IsValid)
            {
                _context.Add(table);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "Id", "Id", table.RoomId);
            return View(table);
        }

        // GET: Manager/Tables/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var table = await _context.Table.FindAsync(id);
            if (table == null)
            {
                return NotFound();
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "Id", "Name", table.RoomId);
            return View(table);
        }

        // POST: Manager/Tables/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Type,RoomId,NumberOfGuest,Status")] Table table)
        {
            if (id != table.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(table);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TableExists(table.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["RoomId"] = new SelectList(_context.Set<Room>(), "Id", "Name", table.RoomId);
            return View(table);
        }

        // GET: Manager/Tables/Delete/5
        

        // POST: Manager/Tables/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var table = await _context.Table.FindAsync(id);
            _context.Table.Remove(table);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Change_Status(int id,TableStatus type,string url)
        {
            var table = await _context.Table.FindAsync(id);
            table.Status = type;
            _context.Update(table);
            await _context.SaveChangesAsync();
            return Redirect(url);
        }

        private bool TableExists(int id)
        {
            return _context.Table.Any(e => e.Id == id);
        }
    }
}
