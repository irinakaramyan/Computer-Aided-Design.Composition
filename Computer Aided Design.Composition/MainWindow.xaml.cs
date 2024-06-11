using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

using System.Windows;

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace wpfproject
{
    public partial class MainWindow : Window
    {
        private const int NodeSize = 10;
        private List<Node> graph = new List<Node>(); // List to store graph nodes
        private Dictionary<Node, Ellipse> nodeEllipseMap = new Dictionary<Node, Ellipse>(); // Dictionary to map nodes to ellipses
        private Random random = new Random();
        private Ellipse selectedNodeEllipse = null;
        private bool isDragging = false;
        private Point lastMousePosition;

        public MainWindow()
        {
            InitializeComponent();

            // Example graph represented as a list of nodes
            CreateGraph();

            // Apply iterative graph partitioning algorithm to segment the graph
            List<List<Node>> segmentedGroups = IterativeGraphPartitioning(graph, 3); // Number of groups

            // Visualize the segmented groups
            VisualizeSegmentedGroups(segmentedGroups);
        }

        // Define a simple Node class to represent graph nodes
        class Node
        {
            public int X { get; set; }
            public int Y { get; set; }
            public int Group { get; set; }
            public List<Node> Neighbors { get; set; } = new List<Node>(); // List of neighboring nodes
        }

        // Create an example graph with nodes and edges
        private void CreateGraph()
        {
            // Create nodes
            for (int i = 0; i < 10; i++)
            {
                AddNode(random.Next(700), random.Next(500));
            }

            // Add edges between nodes
            foreach (Node node1 in graph)
            {
                foreach (Node node2 in graph)
                {
                    if (node1 != node2 && random.Next(10) < 3) // Connect nodes randomly with a probability of 30%
                    {
                        node1.Neighbors.Add(node2);
                        node2.Neighbors.Add(node1);
                    }
                }
            }
        }

        // Iterative graph partitioning algorithm
        private List<List<Node>> IterativeGraphPartitioning(List<Node> graph, int k)
        {
            // Implement iterative graph partitioning algorithm here
            // For simplicity, we'll randomly assign nodes to groups
            foreach (Node node in graph)
            {
                node.Group = random.Next(k) + 1; // Assign node to a random group
            }

            // Partition the graph into groups based on assigned group numbers
            List<List<Node>> segmentedGroups = new List<List<Node>>();
            for (int i = 1; i <= k; i++)
            {
                List<Node> group = new List<Node>();
                foreach (Node node in graph)
                {
                    if (node.Group == i)
                        group.Add(node);
                }
                segmentedGroups.Add(group);
            }

            return segmentedGroups;
        }

        // Visualize segmented groups on the Canvas
        private void VisualizeSegmentedGroups(List<List<Node>> segmentedGroups)
        {
            // Define colors for visualization
            Color[] colors = { Colors.Red, Colors.Blue, Colors.Green, Colors.Yellow, Colors.Magenta, Colors.Cyan };

            int colorIndex = 0;
            foreach (var group in segmentedGroups)
            {
                Color color = colors[colorIndex++ % colors.Length]; // Rotate through the color array

                foreach (var node in group)
                {
                    Ellipse ellipse = new Ellipse
                    {
                        Width = NodeSize,
                        Height = NodeSize,
                        Fill = new SolidColorBrush(color),
                        Stroke = Brushes.Black,
                        StrokeThickness = 2
                    };
                    Canvas.SetLeft(ellipse, node.X);
                    Canvas.SetTop(ellipse, node.Y);
                    nodeEllipseMap[node] = ellipse;
                    GraphCanvas.Children.Add(ellipse);
                    ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
                    ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
                    ellipse.MouseMove += Ellipse_MouseMove;
                }
            }
        }

        // Handle mouse down event on ellipse to start dragging
        private void Ellipse_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            selectedNodeEllipse = (Ellipse)sender;
            isDragging = true;
            lastMousePosition = e.GetPosition(GraphCanvas);
        }

        // Handle mouse up event to stop dragging
        private void Ellipse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            isDragging = false;
        }

        // Handle mouse move event to drag the ellipse
        private void Ellipse_MouseMove(object sender, MouseEventArgs e)
        {
            if (isDragging && selectedNodeEllipse != null)
            {
                Point newMousePosition = e.GetPosition(GraphCanvas);
                double offsetX = newMousePosition.X - lastMousePosition.X;
                double offsetY = newMousePosition.Y - lastMousePosition.Y;
                double newX = Canvas.GetLeft(selectedNodeEllipse) + offsetX;
                double newY = Canvas.GetTop(selectedNodeEllipse) + offsetY;

                Canvas.SetLeft(selectedNodeEllipse, newX);
                Canvas.SetTop(selectedNodeEllipse, newY);

                lastMousePosition = newMousePosition;
            }
        }

        // Add a new node to the graph and update visualization
        private void AddNode(int x, int y)
        {
            Node newNode = new Node { X = x - NodeSize / 2, Y = y - NodeSize / 2 };
            graph.Add(newNode);

            Ellipse ellipse = new Ellipse
            {
                Width = NodeSize,
                Height = NodeSize,
                Fill = Brushes.White,
                Stroke = Brushes.Black,
                StrokeThickness = 2
            };

            Canvas.SetLeft(ellipse, newNode.X);
            Canvas.SetTop(ellipse, newNode.Y);

            nodeEllipseMap[newNode] = ellipse;
            GraphCanvas.Children.Add(ellipse);
            ellipse.MouseLeftButtonDown += Ellipse_MouseLeftButtonDown;
            ellipse.MouseLeftButtonUp += Ellipse_MouseLeftButtonUp;
            ellipse.MouseMove += Ellipse_MouseMove;
        }
    }
}
