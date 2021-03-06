﻿#region License

// Copyright (c) 2009, ClearCanvas Inc.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, 
// are permitted provided that the following conditions are met:
//
//    * Redistributions of source code must retain the above copyright notice, 
//      this list of conditions and the following disclaimer.
//    * Redistributions in binary form must reproduce the above copyright notice, 
//      this list of conditions and the following disclaimer in the documentation 
//      and/or other materials provided with the distribution.
//    * Neither the name of ClearCanvas Inc. nor the names of its contributors 
//      may be used to endorse or promote products derived from this software without 
//      specific prior written permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" 
// AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, 
// THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR 
// PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR 
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, 
// OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE 
// GOODS OR SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) 
// HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, 
// STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN 
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY 
// OF SUCH DAMAGE.

#endregion

#if UNIT_TESTS

#pragma warning disable 1591

using System;
using NUnit.Framework;

namespace ClearCanvas.Common.Specifications.Tests
{
	[TestFixture]
	public class CompositeSpecificationTests : TestsBase
	{
		[Test]
		public void Test_And_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			AndSpecification s = new AndSpecification();
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_And_AllTrue()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_And_AllFalse()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_And_Mixed()
		{
			AndSpecification s = new AndSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			OrSpecification s = new OrSpecification();
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_AllTrue()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_AllFalse()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Or_Mixed()
		{
			OrSpecification s = new OrSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_Degenerate()
		{
			// TODO: this is the current behaviour - perhaps it should throw an exception instead
			NotSpecification s = new NotSpecification();
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_AllTrue()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysTrue);
			s.Add(AlwaysTrue);
			Assert.IsFalse(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_AllFalse()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysFalse);
			Assert.IsTrue(s.Test(null).Success);
		}

		[Test]
		public void Test_Not_Mixed()
		{
			NotSpecification s = new NotSpecification();
			s.Add(AlwaysFalse);
			s.Add(AlwaysTrue);
			Assert.IsTrue(s.Test(null).Success);
		}
	}
}

#endif
