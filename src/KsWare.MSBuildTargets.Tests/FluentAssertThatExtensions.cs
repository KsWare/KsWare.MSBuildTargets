using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace KsWare.MSBuildTargets.Tests {

	public static class FluentAssertThatExtensions {

		public static T Variable<T>(this Assert assert, T value) { return value; }

		public static string String(this Assert assert, string s) { return s; }

		public static int Integer(this Assert assert, int number) { return number; }

		public static double Double(this Assert assert, double number) { return number; }

		public static float Float(this Assert assert, float number) { return number; }
	}
}
