#region License

// Copyright (c) 2006-2007, ClearCanvas Inc.
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

#if	UNIT_TESTS

#pragma warning disable 1591,0419,1574,1587

using System;
using NUnit.Framework;

namespace ClearCanvas.ImageViewer.Tests
{
	[TestFixture]
	public class SelectionTest
	{
		public SelectionTest()
		{
		}

		[TestFixtureSetUp]
		public void Init()
		{
		}
		
		[TestFixtureTearDown]
		public void Cleanup()
		{
		}

		[Test]
		public void SelectTilesInSameImageBox()
		{
			TestTree testTree = new TestTree();

			testTree.Tile1.Select();
			Assert.AreEqual(testTree.Tile1, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image1.Selected);
			Assert.IsTrue(testTree.Tile1.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);

			testTree.Tile2.Select();

			Assert.AreEqual(testTree.Tile2, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image2.Selected);
			Assert.IsTrue(testTree.Tile2.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image1.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile1.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);
		}

		[Test]
		public void SelectTilesInDiffferentImageBoxes()
		{
			TestTree testTree = new TestTree();

			testTree.Tile1.Select();
			Assert.AreEqual(testTree.Tile1, testTree.ImageBox1.SelectedTile);
			Assert.AreEqual(testTree.ImageBox1, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image1.Selected);
			Assert.IsTrue(testTree.Tile1.Selected);
			Assert.IsTrue(testTree.DisplaySet1.Selected);
			Assert.IsTrue(testTree.ImageBox1.Selected);

			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image3.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile3.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet2.Selected);
			Assert.IsFalse(testTree.ImageBox2.Selected);

			testTree.Tile3.Select();

			Assert.AreEqual(testTree.Tile3, testTree.ImageBox2.SelectedTile);
			Assert.AreEqual(testTree.ImageBox2, testTree.Viewer.PhysicalWorkspace.SelectedImageBox);
			Assert.IsTrue(testTree.Image3.Selected);
			Assert.IsTrue(testTree.Tile3.Selected);
			Assert.IsTrue(testTree.DisplaySet2.Selected);
			Assert.IsTrue(testTree.ImageBox2.Selected);

			Assert.IsFalse(testTree.Image1.Selected);
			Assert.IsFalse(testTree.Image2.Selected);
			Assert.IsFalse(testTree.Image4.Selected);
			Assert.IsFalse(testTree.Tile1.Selected);
			Assert.IsFalse(testTree.Tile2.Selected);
			Assert.IsFalse(testTree.Tile4.Selected);

			Assert.IsFalse(testTree.DisplaySet1.Selected);
			Assert.IsFalse(testTree.ImageBox1.Selected);
		}

		[Test]
		[ExpectedException(typeof(InvalidOperationException))]
		public void SelectTileBeforeAddingToTree()
		{
			ITile tile = new Tile();
			tile.Select();
		}
	}
}

#endif