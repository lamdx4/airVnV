using CloudinaryDotNet;
using System.Net.Http;
using System;
using System.Reflection;

class Program {
    static void Main() {
        var c = new Cloudinary("cloudinary://a:b@c");
        Console.WriteLine(c.Api.GetType().GetProperty("Client") != null ? "Has Client Property" : "No Client Property");
        Console.WriteLine(c.Api.Client.GetType().Name);
    }
}
