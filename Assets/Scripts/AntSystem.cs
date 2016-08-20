using System;
using System.Collections.Generic;

namespace JH.ACO {

    public class AntSystem {

        public static Random randomGenerator = new Random();

        private int numberOfNodes;

        public int NumberOfNodes {
            get {
                return numberOfNodes;
            }
        }

        private List<Ant> ants = new List<Ant>();

        public int NumberOfAnts {
            get {
                return ants.Count;
            }
        }

        private double initialPheromone;

        private double functionConstant;

        public double FunctionConstant {
            get {
                return functionConstant;
            }
        }

        private double evaporationRate;

        private double alpha;

        public double Alpha {
            get {
                return alpha;
            }
        }

        private double beta;

        public double Beta {
            get {
                return beta;
            }
        }

        private double[,] pathCosts;

        public double[,] PathCosts {
            get {
                return pathCosts;
            }
        }

        private double[,] pheromoneCosts;

        public double[,] PheromoneCosts {
            get {
                return pheromoneCosts;
            }
        }

        public AntSystem(int numberOfNodes, int numberOfAnts, double initialPheromone, double functionConstant, double evaporationRate, double alpha, double beta) {
            this.numberOfNodes = numberOfNodes;
            this.initialPheromone = initialPheromone;
            this.functionConstant = functionConstant;
            this.evaporationRate = evaporationRate;
            this.alpha = alpha;
            this.beta = beta;

            this.pathCosts = new double[numberOfNodes, numberOfNodes];
            this.pheromoneCosts = new double[numberOfNodes, numberOfNodes];

            for (int index = 0; index < numberOfAnts; index++) {
                this.ants.Add(new Ant(this));
            }
        }

        private void Init() {
            for (int index1 = 0; index1 < numberOfNodes; index1++) {
                for (int index2 = 0; index2 < numberOfNodes; index2++) {
                    this.pheromoneCosts[index1, index2] = randomGenerator.NextDouble() * initialPheromone;
                }
            }
        }

        public void AddPathCost(int nodeFromIndex, int nodeToIndex, double cost) {
            this.pathCosts[nodeFromIndex, nodeToIndex] = cost;
            this.pathCosts[nodeToIndex, nodeFromIndex] = cost;
        }

        public AntSystemResult Execute(int maxSteps, int initialNodeIndex) {
            Init();

            AntSystemResult result = new AntSystemResult();

            double minSolutionQuality = double.MaxValue;
            AntResult bestAntResult = null;

            int stepCounter = 0;

            while (stepCounter < maxSteps) {
                AntSystemResultIteration resultIteration = new AntSystemResultIteration(numberOfNodes);
                result.iterations.Add(resultIteration);

                for (int antIndex = 0; antIndex < ants.Count; antIndex++) {
                    Ant currentAnt = ants[antIndex];
                    currentAnt.Clear();
                    currentAnt.CurrentNodeIndex = initialNodeIndex;

                    while (currentAnt.VisitedPath.Count != numberOfNodes) {
                        currentAnt.PickNextNode();
                    }

                    currentAnt.CalcSolutionQuality();

                    AntResult antResult = new AntResult(numberOfNodes, currentAnt.VisitedPath);
                    resultIteration.ants.Add(antResult);

                    if (currentAnt.SolutionQuality < minSolutionQuality) {
                        minSolutionQuality = currentAnt.SolutionQuality;
                        bestAntResult = antResult;
                    }
                }

                for (int index1 = 0; index1 < numberOfNodes; index1++) {
                    for (int index2 = 0; index2 < numberOfNodes; index2++) {
                        if (index1 != index2) {
                            this.pheromoneCosts[index1, index2] *= (1 - this.evaporationRate);
                            resultIteration.afterEvaporation[index1, index2] = this.pheromoneCosts[index1, index2];

                            double depositedPheromone = 0;
                            for (int antIndex = 0; antIndex < ants.Count; antIndex++) {
                                double pheromoneLevel = this.ants[antIndex].GetDepositedPheromone(index1, index2);
                                depositedPheromone += pheromoneLevel;

                                resultIteration.ants[antIndex].trails[index1, index2] = pheromoneLevel;
                            }

                            this.pheromoneCosts[index1, index2] += depositedPheromone;

                            if (result.maxPheromone < this.pheromoneCosts[index1, index2]) {
                                result.maxPheromone = this.pheromoneCosts[index1, index2];
                            }
                        }
                    }
                }

                stepCounter++;
            }

            result.bestPath = bestAntResult.visitedPath;

            return result;
        }

    }

    public class AntSystemResult {

        public List<AntSystemResultIteration> iterations = new List<AntSystemResultIteration>();

        public double maxPheromone;

        public List<int> bestPath;

    }

    public class AntSystemResultIteration {

        public List<AntResult> ants = new List<AntResult>();

        public double[,] afterEvaporation;

        public AntSystemResultIteration(int numberOfNodes) {
            this.afterEvaporation = new double[numberOfNodes, numberOfNodes];
        }

    }

    public class AntResult {

        public List<int> visitedPath;

        public double[,] trails;

        public AntResult(int numberOfNodes, List<int> visitedPath) {
            this.trails = new double[numberOfNodes, numberOfNodes];
            this.visitedPath = visitedPath;
        }

    }

}