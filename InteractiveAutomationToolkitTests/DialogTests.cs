namespace InteractiveAutomationToolkitTests
{
	using System;

	using Microsoft.VisualStudio.TestTools.UnitTesting;

	using Moq;

	using Skyline.DataMiner.Automation;
	using Skyline.DataMiner.DeveloperCommunityLibrary.InteractiveAutomationToolkit;

	[TestClass]
	public class DialogTests
	{
		/// <summary>
		/// This test will add two different labels (without spanning) on the same location and check if an exception is thrown.
		/// </summary>
		[TestMethod]
		public void TryAddWidgetsSameLocation()
		{
			Label label1 = new Label("Label 1");
			Label label2 = new Label("Label 2");
			Dialog dialog = new Dialog(Mock.Of<IEngine>());
			dialog.AddWidget(label1, 0, 0);
			dialog.AddWidget(label2, 0, 0);

			Assert.ThrowsException<OverlappingWidgetsException>(() => dialog.Show(false));
		}

		/// <summary>
		/// This test will add two different overlapping labels (with column spanning) to the dialog and check if an exception is thrown.
		/// </summary>
		[TestMethod]
		public void TryAddOverlappingColumnSpanningWidgets()
		{
			Label label1 = new Label("Label 1");
			Label label2 = new Label("Label 2");
			Dialog dialog = new Dialog(Mock.Of<IEngine>());
			dialog.AddWidget(label1, 0, 0, 1, 3);
			dialog.AddWidget(label2, 0, 2, 1, 2);

			Assert.ThrowsException<OverlappingWidgetsException>(() => dialog.Show(false));
		}

		/// <summary>
		/// This test will add two different overlapping labels (with row spanning) to the dialog and check if an exception is thrown.
		/// </summary>
		[TestMethod]
		public void TryAddOverlappingRowSpanningWidgets()
		{
			Label label1 = new Label("Label 1");
			Label label2 = new Label("Label 2");
			Dialog dialog = new Dialog(Mock.Of<IEngine>());
			dialog.AddWidget(label1, 0, 0, 3, 1);
			dialog.AddWidget(label2, 2, 0, 2, 1);

			Assert.ThrowsException<OverlappingWidgetsException>(() => dialog.Show(false));
		}

		/// <summary>
		/// This test will add two different overlapping labels (with column and row spanning) to the dialog and check if an exception is thrown.
		/// </summary>
		[TestMethod]
		public void TryAddOverlappingColumnAndRowSpanningWidgets()
		{
			Label label1 = new Label("Label 1");
			Label label2 = new Label("Label 2");
			Dialog dialog = new Dialog(Mock.Of<IEngine>());
			dialog.AddWidget(label1, 0, 0, 2, 2);
			dialog.AddWidget(label2, 1, 1, 2, 3);

			Assert.ThrowsException<OverlappingWidgetsException>(() => dialog.Show(false));
		}

		/// <summary>
		/// This test will add two different overlapping invisible labels (with column and row spanning) to the dialog and check if no exception is thrown.
		/// </summary>
		[TestMethod]
		public void TryAddOverlappingInvisibleColumnAndRowSpanningWidgets()
		{
			Label label1 = new Label("Label 1");
			Label label2 = new Label("Label 2") { IsVisible = false };
			Dialog dialog = new Dialog(Mock.Of<IEngine>());
			dialog.AddWidget(label1, 0, 0, 2, 2);
			dialog.AddWidget(label2, 1, 1, 2, 3);

			try
			{
				dialog.Show(false);
			}
			catch (Exception ex)
			{
				Assert.Fail("Expected no exception, but got: " + ex.Message);
			}
		}
	}
}
