﻿using CjClutter.Commons.Extensions;
using CjClutter.OpenGl.CoordinateSystems;
using NUnit.Framework;
using OpenTK;
using Rhino.Mocks;
using FluentAssertions;

namespace ObjLoader.Test.CoordinateSystems
{
    [TestFixture]
    public class GuiToRelativeCoordinateTransformerTests
    {
        private GuiToRelativeCoordinateTransformer _transformer;
        private IInterfaceSize _interfaceSize;

        [SetUp]
        public void SetUp()
        {
            _transformer = new GuiToRelativeCoordinateTransformer();
            
            _interfaceSize = MockRepository.GenerateMock<IInterfaceSize>();
            _interfaceSize.Stub(x => x.Width).Return(400);
            _interfaceSize.Stub(x => x.Height).Return(300);

            _transformer.Interface = _interfaceSize;
        }

        [Test]
        public void Transforms_center_x_coordinate_from_absolute_gui_coordinate_to_relative_coordinate()
        {
            var absoluteCoordinate = new Vector2d(200, 0);
            var actualCoordinate = _transformer.TransformToRelative(absoluteCoordinate);

            var expectedCoordinate = new Vector2d(0, 0);

            actualCoordinate.X.Should().BeApproximately(expectedCoordinate.X);
        }

        [Test]
        public void Transforms_left_x_coordinate_from_absolute_gui_coordinate_to_relative_coordinate()
        {
            var absoluteCoordinate = new Vector2d(150, 0);
            var actualCoordinate = _transformer.TransformToRelative(absoluteCoordinate);

            var expectedCoordinate = new Vector2d(-0.25, 0);
            actualCoordinate.X.Should().BeApproximately(expectedCoordinate.X);
        }

        [Test]
        public void Transforms_right_x_coordinate_from_absolute_gui_coordinate_to_relative_coordinate()
        {
            var absoluteCoordinate = new Vector2d(250, 0);
            var actualCoordinate = _transformer.TransformToRelative(absoluteCoordinate);

            var expectedCoordinate = new Vector2d(0.25, 0);
            actualCoordinate.X.Should().BeApproximately(expectedCoordinate.X);
        }

        [Test]
        public void Transforms_center_y_coordinate_from_absolute_gui_coordinate_to_relative_coordinate()
        {
            var absoluteCoordinate = new Vector2d(0, 150);
            var actualCoordinate = _transformer.TransformToRelative(absoluteCoordinate);

            var expectedCoordinate = new Vector2d(0, 0);

            actualCoordinate.Y.Should().BeApproximately(expectedCoordinate.Y);
        }

        [Test]
        public void Transforms_top_y_coordinate_from_absolute_gui_coordinate_to_relative_coordinate()
        {
            var absoluteCoordinate = new Vector2d(0, 100);
            var actualCoordinate = _transformer.TransformToRelative(absoluteCoordinate);

            var expectedCoordinate = new Vector2d(0, 1/3.0);
            actualCoordinate.Y.Should().BeApproximately(expectedCoordinate.Y);
        }

        [Test]
        public void Transforms_bottom_y_coordinate_from_absolute_gui_coordinate_to_relative_coordinate()
        {
            var absoluteCoordinate = new Vector2d(0, 200);
            var actualCoordinate = _transformer.TransformToRelative(absoluteCoordinate);

            var expectedCoordinate = new Vector2d(0, -1/3.0);
            actualCoordinate.Y.Should().BeApproximately(expectedCoordinate.Y);
        }
    }
}