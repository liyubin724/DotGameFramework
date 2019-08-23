using System;
using UnityEngine;

namespace AddressablesSystemExtend
{
	[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
	public sealed class AddressablesSystemEditorAttribute : PropertyAttribute
	{
	}
}