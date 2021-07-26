/*
    AvaloniaAccordion - An accordion-like expandable control for Avalonia.
    Copyright (C) 2021  Giorgio Bianchini
 
    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, version 3.
    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.
    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/

using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Markup.Xaml;
using Avalonia.Media;
using Avalonia.VisualTree;
using System;

namespace AvaloniaAccordion
{
    public partial class Accordion : UserControl
    {
        /// <summary>
        /// If the accordion is open, forces a recomputation of the height to make sure that all the contents fit.
        /// </summary>
        public void InvalidateHeight()
        {
            if (this.IsOpen)
            {
                AccordionHeightConverter.ReturnLastValue = true;
                AccordionAngleConverter.ReturnLastValue = true;
                this.IsOpen = false;

                _ = Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                    {
                        AccordionHeightConverter.ReturnLastValue = false;
                        AccordionAngleConverter.ReturnLastValue = false;
                        this.IsOpen = true;

                        Accordion parentAccordion = this.FindAncestorOfType<Accordion>();

                        if (parentAccordion != null)
                        {
                            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(Math.Max(10, this.TransitionDuration.TotalMilliseconds * 2)));
                            parentAccordion.InvalidateHeight();
                            await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(Math.Max(10, this.TransitionDuration.TotalMilliseconds)));
                            parentAccordion.InvalidateHeight();
                        }

                    }, Avalonia.Threading.DispatcherPriority.MinValue);
            }
        }


        private Grid ContentGridParent;
        private AccordionHeightConverter AccordionHeightConverter;
        private AccordionAngleConverter AccordionAngleConverter;

        /// <inheritdoc/>
        protected override void OnPropertyChanged<T>(AvaloniaPropertyChangedEventArgs<T> change)
        {
            base.OnPropertyChanged(change);

            if (change.Property == Accordion.TransitionDurationProperty)
            {
                if (ContentGridParent != null)
                {
                    ((Avalonia.Animation.DoubleTransition)ContentGridParent.Transitions[0]).Duration = change.NewValue.GetValueOrDefault<TimeSpan>();

                    ((Avalonia.Animation.DoubleTransition)((RotateTransform)this.FindControl<Path>("ArrowPathLeft").RenderTransform).Transitions[0]).Duration = change.NewValue.GetValueOrDefault<TimeSpan>();
                    ((Avalonia.Animation.DoubleTransition)((RotateTransform)this.FindControl<Path>("ArrowPathRight").RenderTransform).Transitions[0]).Duration = change.NewValue.GetValueOrDefault<TimeSpan>();
                }
            }
            else if (change.Property == Accordion.AccordionContentProperty)
            {
                this.InvalidateHeight();
            }
            else if (change.Property == Accordion.BottomBarThicknessProperty)
            {
                if (this.IsOpen)
                {
                    this.FindControl<Canvas>("BottomCanvas").RenderTransform = null;
                }
                else
                {
                    this.FindControl<Canvas>("BottomCanvas").RenderTransform = new TranslateTransform(0, -change.NewValue.GetValueOrDefault<double>());
                }
            }
            else if (change.Property == Accordion.IsOpenProperty)
            {
                if (change.NewValue.GetValueOrDefault<bool>())
                {
                    this.FindControl<Canvas>("BottomCanvas").RenderTransform = null;
                }
                else
                {
                    this.FindControl<Canvas>("BottomCanvas").RenderTransform = new TranslateTransform(0, -this.BottomBarThickness);
                }

                Avalonia.Threading.Dispatcher.UIThread.InvokeAsync(async () =>
                {
                    Accordion parentAccordion = this.FindAncestorOfType<Accordion>();

                    if (parentAccordion != null)
                    {
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(Math.Max(10, this.TransitionDuration.TotalMilliseconds * 2)));
                        parentAccordion.InvalidateHeight();
                        await System.Threading.Tasks.Task.Delay(TimeSpan.FromMilliseconds(Math.Max(10, this.TransitionDuration.TotalMilliseconds)));
                        parentAccordion.InvalidateHeight();
                    }
                });
            }
        }

        /// <summary>
        /// Creates a new <see cref="Accordion"/> instance.
        /// </summary>
        public Accordion()
        {
            InitializeComponent();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);

            this.FindControl<Canvas>("BottomCanvas").RenderTransform = new TranslateTransform(0, -this.BottomBarThickness);

            this.AccordionHeightConverter = ((AccordionHeightConverter)this.FindResource("AccordionHeightConverter"));

            AccordionHeightConverter.ContentGrid = this.FindControl<Grid>("ContentGrid");

            this.AccordionAngleConverter = ((AccordionAngleConverter)this.FindResource("AccordionAngleConverter"));

            this.ContentGridParent = this.FindControl<Grid>("ContentGridParent");

            ((Avalonia.Animation.DoubleTransition)ContentGridParent.Transitions[0]).Duration = this.TransitionDuration;
            ((Avalonia.Animation.DoubleTransition)((RotateTransform)this.FindControl<Path>("ArrowPathLeft").RenderTransform).Transitions[0]).Duration = this.TransitionDuration;
            ((Avalonia.Animation.DoubleTransition)((RotateTransform)this.FindControl<Path>("ArrowPathRight").RenderTransform).Transitions[0]).Duration = this.TransitionDuration;

            this.FindControl<Grid>("HeaderGrid").PointerPressed += (s, e) =>
            {
                this.FindControl<Grid>("HeaderGrid").Classes.Add("Pressed");
            };

            this.FindControl<Grid>("HeaderGrid").PointerCaptureLost += (s, e) =>
            {
                this.FindControl<Grid>("HeaderGrid").Classes.Remove("Pressed");
            };

            this.FindControl<Grid>("HeaderGrid").PointerReleased += (s, e) =>
            {
                this.FindControl<Grid>("HeaderGrid").Classes.Remove("Pressed");

                this.IsOpen = !this.IsOpen;
            };
        }
    }
}
