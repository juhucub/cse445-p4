using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;



/**
 * This template file is created for ASU CSE445 Distributed SW Dev Assignment 4.
 * Please do not modify or delete any existing class/variable/method names. However, you can add more variables and functions.
 * Uploading this file directly will not pass the autograder's compilation check, resulting in a grade of 0.
 * **/


namespace ConsoleApp1
{


    public class Program
    {
        public static string xmlURL = "https://juhucub.github.io/cse445-p4/Hotels.xml";
        public static string xmlErrorURL = "https://juhucub.github.io/cse445-p4/HotelsErrors.xml";
        public static string xsdURL = "https://juhucub.github.io/cse445-p4/Hotels.xsd";

        public static void Main(string[] args)
        {
            string result = Verification(xmlURL, xsdURL);
            Console.WriteLine(result);


            result = Verification(xmlErrorURL, xsdURL);
            Console.WriteLine(result);


            result = Xml2Json(xmlURL);
            Console.WriteLine(result);
        }

        // Q2.1 - return "No Error" if XML is valid. Otherwise, return the desired exception message.
        public static string Verification(string xmlUrl, string xsdUrl)
        {
            //.NET provides  XML validation using XmlSchemaSet
            var errors = new List<string>();

            //Build Schema set from XSD url
            var schema = new XmlSchemaSet();
            //Pass Hotels.xsd target namespace
            const string targetNamespace = "http://venus.sod.asu.edu/WSRepository/xml";
            //.NET grabs XSD from URL over HTTPS
            schema.Add(targetNamespace, xsdUrl);

            //Setup XmlReaderSettings for validation
            var reader = new XmlReaderSettings()
            {
                ValidationType = ValidationType.Schema,
                Schemas = schema,
                DtdProcessing = DtdProcessing.Prohibit,
                IgnoreComments = true,
                IgnoreWhitespace = true
            };
            reader.ValidationEventHandler += (sender, e) =>
            {
                errors.Add(e.Message);
            };

            //Read XML from URL over HTTPS and validate
            try
            {
                using (var xmlReader = XmlReader.Create(xmlUrl, reader))
                {
                    while (xmlReader.Read()) { }
                }
            }
            catch (XmlException ex)
            {
                errors.Add(ex.Message);
            }
            catch (Exception x)
            {
                errors.Add(x.Message);
            }
            if (errors.Count == 0)
            {
                return "No errors are found";
            }
            else
            {
                return string.Join("\n", errors);
            }
        }

        public static string Xml2Json(string xmlUrl)
        {
            //Load XML from URL
            var xmlDoc = XDocument.Load(xmlUrl);
            XNamespace link = "http://venus.sod.asu.edu/WSRepository/xml";

            //helper to read all phones for a hotel
            List<string> GetPhones(XElement hotel) => hotel.Elements(link + "Phone")
               .Select(p => (p.Value ?? "").Trim())
               .Where(v => v.Length > 0)
               .ToList();

            //Build each hotel before converting to json
            var hotels = xmlDoc.Root.Elements(link + "Hotel")
                         .Select(hotel =>
                         {
                             var address = hotel.Element(link + "Address");

                             return new
                             {
                                 Name = (string)hotel.Element(link + "Name"),
                                 Phone = GetPhones(hotel),
                                 Address = new
                                 {
                                     Number = (string)address.Element(link + "Number"),
                                     Street = (string)address.Element(link + "Street"),
                                     City = (string)address.Element(link + "City"),
                                     State = (string)address.Element(link + "State"),
                                     Zip = (string)address.Element(link + "Zip"),
                                     NearestAirport = (string)address.Attribute("NearestAirport") ?? ""
                                 },
                                 _Rating = (string)hotel.Attribute("Rating")
                             };
                         }) . ToList();

            //wrap in a Hotels root
            var HotelsRoot = new
            {
                Hotels = new
                {
                    Hotel = hotels
                }
            };

            //Serialize to JSON and ignore nulls
            return JsonConvert.SerializeObject(HotelsRoot, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
            });


        }
    }

}
