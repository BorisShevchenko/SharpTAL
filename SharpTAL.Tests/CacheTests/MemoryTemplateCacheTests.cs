﻿using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Reflection;

using NUnit.Framework;

namespace SharpTAL.SharpTALTests.CacheTests
{
	[TestFixture]
	public class MemoryTemplateCacheTests
	{
		public static SharpTAL.MemoryTemplateCache cache;
		public static Dictionary<string, object> globals;
		public static List<Assembly> refAssemblies;

		[TestFixtureSetUp]
		public void SetUpClass()
		{
			cache = new MemoryTemplateCache();
		}

		[TestFixtureTearDown]
		public void CleanupClass()
		{
		}

		[SetUp]
		public void SetUp()
		{
			globals = new Dictionary<string, object>();
			globals.Add("the_string", "string");

			refAssemblies = new List<Assembly>();
			refAssemblies.Add(typeof(MemoryTemplateCacheTests).Assembly);
		}

		[Test]
		public void TestReuseTemplateInfo()
		{
			TemplateInfo ti;

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash1 = ti.TemplateKey;

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash2 = ti.TemplateKey;
			Assert.AreEqual(hash1, hash2, "Reusing generated template in cache failed. Hash1: {0}, Hash2: {1}", hash1, hash2);
		}

		[Test]
		public void TestTemplateChange()
		{
			TemplateInfo ti;

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash1 = ti.TemplateKey;

			cache.RenderTemplate(@"<template2></template2>", globals, refAssemblies, out ti);
			string hash2 = ti.TemplateKey;
			Assert.AreNotEqual(hash1, hash2, "Template re-generation if template change failed. Hash1: {0}, Hash2: {1}", hash1, hash2);
		}

		[Test]
		public void TestGlobalsChange()
		{
			TemplateInfo ti;

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash1 = ti.TemplateKey;

			globals.Add("the_string_2", "string");

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash2 = ti.TemplateKey;
			Assert.AreNotEqual(hash1, hash2, "Template re-generation if globals types change failed. Hash1: {0}, Hash2: {1}", hash1, hash2);
		}

		[Test]
		public void TestRefAssembliesChange()
		{
			TemplateInfo ti;

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash1 = ti.TemplateKey;

			refAssemblies.Clear();

			cache.RenderTemplate(@"<template></template>", globals, refAssemblies, out ti);
			string hash2 = ti.TemplateKey;
			Assert.AreNotEqual(hash1, hash2, "Template re-generation if referenced assemblies change failed. Hash1: {0}, Hash2: {1}", hash1, hash2);
		}

		[Test]
		public void TestImportChange()
		{
			TemplateInfo ti;
			string templateBody = @"<template metal:import='TestImportChange_Imports.xml'></template>";

			File.WriteAllText("TestImportChange_Imports.xml", "<import1></import1>");

			cache.RenderTemplate(templateBody, globals, refAssemblies, out ti);
			string hash1 = ti.TemplateKey;

			File.WriteAllText("TestImportChange_Imports.xml", "<import2></import2>");

			cache.RenderTemplate(templateBody, globals, refAssemblies, out ti);
			string hash2 = ti.TemplateKey;
			Assert.AreNotEqual(hash1, hash2, "Template re-generation if referenced assemblies change failed. Hash1: {0}, Hash2: {1}", hash1, hash2);
		}
	}
}
