using System;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using System.Linq;
using System.Collections.Generic;
using VatsimLibrary.VatsimClient;
using VatsimLibrary.VatsimData;
using VatsimLibrary.VatsimDb;

namespace hw
{
    class Program
    {
        static void Main(string[] args)
        {
            using(var db = new VatsimDbContext())
                {
                    //controller
                    var _controller = db.Controllers;

                    List<VatsimClientATC> c = new List<VatsimClientATC>();               

                    foreach(var controller in _controller){
                        c.Add(controller);
                    }

                    //pilot
                    var _pilot = db.Pilots;

                    List<VatsimClientPilot> p = new List<VatsimClientPilot>();

                    foreach(var pilot in _pilot){
                        p.Add(pilot);
                    }

                    //flight
                    var _flight = db.Flights;

                    List<VatsimClientPlannedFlight> f = new List<VatsimClientPlannedFlight>();

                    foreach(var flight in _flight){
                        f.Add(flight);
                    }

                    //position
                    var _position = db.Positions;

                    List<VatsimClientPilotSnapshot> ps = new List<VatsimClientPilotSnapshot>();

                    foreach(var position in _position){
                        ps.Add(position);
                    }

                    //FOR REFERENCE: select all controllers
                    var cQuery = from con in c select con;

                    //FOR REFERENCE: select only callsigns from client
                    var callS = from var17 in p select var17.Callsign;
                    foreach(var sign in callS) {
                        //Console.WriteLine(sign);
                    }

                    //query 1
                    Console.WriteLine("Starting query 1...");

                    var query1 =
                    (from record in p
                    orderby record.TimeLogon
                    select record).Take(1);

                    foreach (var item1 in query1)
                    {
                        Console.WriteLine("The pilot who has been logged on the longest is named " + item1.Realname + " and has callsign " + item1.Callsign );
                    }

                    Console.WriteLine("");

                    //query 2
                    Console.WriteLine("Starting query 2...");

                    var query2 =
                    (from record in c
                    orderby record.TimeLogon
                    select record).Take(1);

                    foreach (var item2 in query2)
                    {
                        Console.WriteLine("The controller who has been logged on the longest is named " + item2.Realname + " and has callsign " + item2.Callsign );
                    }

                    Console.WriteLine("");

                    //query 3
                    Console.WriteLine("Starting query 3...");

                    var query3 =
                    (from record in f
                    group record by record.PlannedDepairport into depPort
                    orderby depPort.Count() descending
                    select new { depPort.Key, Count = depPort.Count() }).Take(1);

                    foreach (var item3 in query3)
                    {
                        Console.WriteLine("The airport with the most departures is " + item3.Key + " with " + item3.Count + " departures.");
                    }

                    Console.WriteLine("");

                    //query 4
                    Console.WriteLine("Starting query 4...");

                    var query4 =
                    (from record in f
                    group record by record.PlannedDestairport into arrPort
                    orderby arrPort.Count() descending
                    select new { arrPort.Key, Count = arrPort.Count() }).Take(1);

                    foreach (var item4 in query4)
                    {
                        Console.WriteLine("The airport with the most arrivals is " + item4.Key + " with " + item4.Count + " arrivals.");
                    }

                    Console.WriteLine("");


                    //query 5
                    Console.WriteLine("Starting query 5...");

                    var query5 =
                    (from record in f
                    orderby record.PlannedAltitude descending
                    where record.PlannedAltitude.ToString().Length == 5 //note: i know this is iffy code, but linq doesn't do natural sorting, so it would list 9,000ft as higher than 50,000 without this line. we can assume that at least one person is flying in a 5 digit number
                    select new { record.PlannedAltitude, planeType = record.PlannedAircraft }).Take(1);

                    foreach (var item5 in query5)
                    {
                        Console.WriteLine("The plane flying the highest is at " + item5.PlannedAltitude + "ft, and is a " + item5.planeType);
                    }

                    Console.WriteLine("");

                    //query 6
                    Console.WriteLine("Starting query 6...");

                    var query6 =
                    (from record in ps
                    orderby record.Groundspeed 
                    where record.Altitude != "0"
                    where record.Groundspeed != "0"
                    select record).Take(1);

                    foreach (var item6 in query6)
                    {
                        Console.WriteLine("The plane flying the slowest is at " + item6.Groundspeed + "knots, and is " + item6.Callsign + ". They are at an altitude of "+ item6.Altitude + " ft");
                    }

                    Console.WriteLine("");

                    //query 7
                    Console.WriteLine("Starting query 7...");

                    var query7 =
                    (from record in f
                    group record by record.PlannedAircraft into mostAc
                    orderby mostAc.Count() descending
                    select new { mostAc.Key, Count = mostAc.Count() }).Take(1);

                    foreach (var item7 in query7)
                    {
                        Console.WriteLine("The the aircraft being used the most is " + item7.Key + " and is being used " + item7.Count + " times.");
                    }

                    Console.WriteLine("");


                    //query 8
                    Console.WriteLine("Starting query 8...");

                    var query8 =
                    (from record in ps
                    orderby record.Groundspeed descending
                    where record.Altitude != "0"
                    where record.Groundspeed != "0"
                    select record).Take(1);

                    foreach (var item8 in query8)
                    {
                        Console.WriteLine("The plane flying the fastest is at " + item8.Groundspeed + "knots, and is " + item8.Callsign + ". They are at an altitude of "+ item8.Altitude + " ft");
                    }

                    Console.WriteLine("");
                    
                    //query 9
                    Console.WriteLine("Starting query 9...");

                    var query9 =
                    (from record in ps
                    where (Int32.Parse(record.Heading)) >= 90 && (Int32.Parse(record.Heading)) <= 270
                    select record);

                    var counted = query9.Count();

                    Console.WriteLine("There are " + counted + " planes flying north (270* to 90*)");
                    Console.WriteLine("");
                    
                    //query 10
                    Console.WriteLine("Starting query 10...");

                    var query10 =
                    (from record in f
                    select record.PlannedRemarks);

                    var sortedValues = query10
                        .OrderByDescending(x => x.Length).Take(1);

                    foreach (var item10 in sortedValues)
                    {
                        Console.WriteLine("The longest remark is: "+ item10);
                    }

                    Console.WriteLine("");

                }
        }
    }
}