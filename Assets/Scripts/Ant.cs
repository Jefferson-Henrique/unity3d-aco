using System;
using System.Collections.Generic;

namespace JH.ACO {

    public class Ant {

        private AntSystem antSystem;

        public AntSystem AntSystem {
            get {
                return antSystem;
            }
        }

        private double solutionQuality;

        public double SolutionQuality {
            get {
                return solutionQuality;
            }
        }

        private List<int> visitedPath = new List<int>();

        public List<int> VisitedPath {
            get {
                return new List<int>(visitedPath);
            }
        }

        private int currentNodeIndex;

        public int CurrentNodeIndex {
            get {
                return currentNodeIndex;
            }

            set {
                currentNodeIndex = value;
                this.visitedPath.Add(currentNodeIndex);
            }
        }

        public Ant(AntSystem antSystem) {
            this.antSystem = antSystem;
        }

        public void Clear() {
            this.solutionQuality = 0;
            this.visitedPath.Clear();
            this.currentNodeIndex = 0;
        }

        public void PickNextNode() {
            List<int> mapNodeIndex = new List<int>();
            List<double> mapNodeValue = new List<double>();

            double denominatorSum = 0;

            for (int nodeIndex = 0; nodeIndex < this.antSystem.NumberOfNodes; nodeIndex++) {
                if (this.visitedPath.IndexOf(nodeIndex) == -1) {
                    double pheromoneValue = Math.Pow(this.antSystem.PheromoneCosts[this.currentNodeIndex, nodeIndex], this.antSystem.Alpha);
                    double heuristicValue = Math.Pow(1 / this.antSystem.PathCosts[this.currentNodeIndex, nodeIndex], this.antSystem.Beta);

                    double totalValue = pheromoneValue * heuristicValue;

                    denominatorSum += totalValue;
                    mapNodeIndex.Add(nodeIndex);
                    mapNodeValue.Add(totalValue);
                }
            }

            double randomValueChosen = AntSystem.randomGenerator.NextDouble() * denominatorSum;
            double probabilityAux = 0;

            int nextNodeIndex = -1;
            for (int index = 0; index < mapNodeIndex.Count; index++) {
                double currentValue = mapNodeValue[index];

                if (randomValueChosen >= probabilityAux && randomValueChosen < (probabilityAux + currentValue)) {
                    nextNodeIndex = mapNodeIndex[index];
                    break;
                }

                probabilityAux += currentValue;
            }

            CurrentNodeIndex = nextNodeIndex;
        }

        public void CalcSolutionQuality() {
            this.solutionQuality = 0;

            for (int index = 0; index < (this.visitedPath.Count - 1); index++) {
                this.solutionQuality += this.antSystem.PathCosts[this.visitedPath[index], this.visitedPath[index+1]];
            }
        }

        public double GetDepositedPheromone(int nodeFromIndex, int nodeToIndex) {
            int visitedIndexFrom = this.visitedPath.IndexOf(nodeFromIndex);

            if ((visitedIndexFrom > 0 && this.visitedPath[visitedIndexFrom - 1] == nodeToIndex) || (visitedIndexFrom < (this.visitedPath.Count - 1) && this.visitedPath[visitedIndexFrom+1] == nodeToIndex)) {
                return this.antSystem.FunctionConstant / this.solutionQuality;
            }

            return 0;
        }

    }

}