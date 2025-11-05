using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Remoting.Contexts;
using System.Xml;
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


            //result = Verification(xmlErrorURL, xsdURL);
            // Console.WriteLine(result);


            //result = Xml2Json(xmlURL);
            //Console.WriteLine(result);
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
                return "No Error";
            }
            else
            {
                return string.Join("\n", errors);
            }
        }

        public static string Xml2Json(string xmlUrl)
        {


            // The returned jsonText needs to be de-serializable by Newtonsoft.Json package. (JsonConvert.DeserializeXmlNode(jsonText))
            return "yeah";

        }
    }

}
