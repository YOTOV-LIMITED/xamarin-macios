using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;

using NUnit.Framework;

#if XAMCORE_2_0
using Foundation;
using ObjCRuntime;
#elif MONOMAC
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#else
using MonoTouch.Foundation;
using MonoTouch.ObjCRuntime;
#endif

namespace TouchUnit.Bindings {
	[Preserve (AllMembers = true)]
	public abstract class ApiWeakPropertyTest : ApiBaseTest {

		/// <summary>
		/// Override if you want to skip testing the specified type.
		/// </summary>
		/// <param name="type">Type to be tested</param>
		protected virtual bool Skip (Type type)
		{
			switch (type.Name) {
				case "LinkWithAttribute": // LinkWithAttribute.WeakFrameworks
				return true;
			}
			return false;
		}

		/// <summary>
		/// Override if you want to skip testing the specified property.
		/// </summary>
		/// <param name="property">Property candidate.</param>
		protected virtual bool Skip (PropertyInfo property)
		{
			return false;
		}

		[Test]
		public void WeakPropertiesHaveArgumentSemantic ()
		{
			var failed_properties = new List<string> ();

			Errors = 0;
			int c = 0, n = 0;
			foreach (Type t in Assembly.GetTypes ()) {
				if (Skip (t) || SkipDueToAttribute (t))
					continue;

				if (LogProgress)
					Console.WriteLine ("{0}. {1}", c++, t.FullName);

				foreach (var p in t.GetProperties (BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)) {
					// looking for properties with setters only
					if (!p.CanWrite)
						continue;

					if (SkipDueToAttribute (p))
						continue;

					if (Skip (p))
						continue;

					string name = p.Name;
					if (!name.StartsWith ("Weak"))
						continue;

					string error;
					if (CheckArgumentSemantic (p.GetMethod, out error)) {
						ReportError (error);
						failed_properties.Add (p.ToString ());
					}
					if (CheckArgumentSemantic (p.SetMethod, out error)) {
						ReportError (error);
						failed_properties.Add (p.ToString ());
					}
					n++;
				}
			}
			Assert.AreEqual (0, Errors, "{0} errors found in {1} fields validated: {2}", Errors, n, string.Join (", ", failed_properties));
		}

		bool CheckArgumentSemantic (MethodInfo meth, out string error)
		{
			error = null;
			var export = meth.GetCustomAttribute<ExportAttribute> ();
			if (export == null) {
				error = String.Format ("{0}.{1} has no [Export]", meth.DeclaringType.FullName, meth.Name);
				return true;
			}

			switch (export.ArgumentSemantic) {
			case ArgumentSemantic.Assign: // Also case ArgumentSemantic.UnsafeUnretained:
			case ArgumentSemantic.Copy:
			case ArgumentSemantic.Retain: // case ArgumentSemantic.Strong:
			case ArgumentSemantic.Weak:
				return false;
			default:
				error = String.Format ("{0}.{1} has incorrect ArgumentSemantics: {2}", meth.DeclaringType.FullName, meth.Name, export.ArgumentSemantic);
				return true;
			}
		}
	}
}