using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using MathNet.Numerics.LinearAlgebra;
using MathNet.Numerics.LinearAlgebra.Double;
using PetTravelGuide.Backend.Database;

namespace YourNamespace
{
    [ApiController]
    [Route("api/recommendations")]
    public class RecommendationController : ControllerBase
    {
        private RecommendationSystem recommendationSystem;
        private DatabaseContext dbContext;

        public RecommendationController(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
            recommendationSystem = new RecommendationSystem(dbContext);
        }

        [HttpGet("{userId}")]
        public ActionResult<List<Item>> GetRecommendations(int userId)
        {
            try
            {
                var recommendations = recommendationSystem.GetRecommendations(userId);
                return Ok(recommendations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"An error occurred: {ex.Message}");
            }
        }
    }

    public class RecommendationSystem
    {
        private List<Data> data;
        private DatabaseContext dbContext;
        private Dictionary<int, Dictionary<int, double>> userPreferences;

        public RecommendationSystem(DatabaseContext dbContext)
        {
            this.dbContext = dbContext;
            this.data = dbContext.Data.ToList();
            this.userPreferences = ComputeUserPreferences();
        }

        public List<Item> GetRecommendations(int userId)
        {
            Console.WriteLine("Fetching user preferences...");
            var userPrefs = userPreferences.ContainsKey(userId) ? userPreferences[userId] : new Dictionary<int, double>();
            Console.WriteLine("Fetching other users...");
            var otherUsers = GetOtherUsers(userId);

            if (!userPrefs.Any() || !otherUsers.Any())
            {
                Console.WriteLine("No data available. Returning an empty list of recommendations.");
                return new List<Item>(); // Return an empty list if no data is available
            }

            Console.WriteLine("Creating ratings matrix...");
            var ratingsMatrix = CreateUserItemMatrix(userPrefs, otherUsers);
            Console.WriteLine("Calculating user similarities...");
            var similarities = CalculateUserSimilarities(ratingsMatrix, userId);

            Console.WriteLine("Sorting top users...");
            var topUsers = similarities.OrderByDescending(kv => kv.Value).Select(kv => kv.Key);

            Console.WriteLine("Generating recommendations...");
            var recommendations = new HashSet<Item>(); // Use a HashSet to store unique items

            foreach (var user in topUsers)
            {
                recommendations.UnionWith(GetUserPreferences(user).Where(p => !userPrefs.ContainsKey(p.Key)).Select(p => GetItemById(p.Key)));
            }

            Console.WriteLine("Recommendations generated successfully.");

            return recommendations.Take(5).ToList(); // Limit the recommendations to 5 items
        }

        private Dictionary<int, double> GetUserPreferences(int userId)
        {
            return userPreferences.ContainsKey(userId) ? userPreferences[userId] : new Dictionary<int, double>();
        }

        private List<int> GetOtherUsers(int userId)
        {
            return data.Where(d => d.UserID != userId).Select(d => d.UserID).Distinct().ToList();
        }

        private Item GetItemById(int itemId)
        {
            return dbContext.Items.FirstOrDefault(i => i.Id == itemId);
        }

        private Dictionary<int, Dictionary<int, double>> ComputeUserPreferences()
        {
            var userPrefs = new Dictionary<int, Dictionary<int, List<double>>>();

            foreach (var datum in data)
            {
                if (!userPrefs.ContainsKey(datum.UserID))
                {
                    userPrefs[datum.UserID] = new Dictionary<int, List<double>>();
                }

                if (!userPrefs[datum.UserID].ContainsKey(datum.LocationID))
                {
                userPrefs[datum.UserID][datum.LocationID] = new List<double> { datum.Rating };
                }
                else
                {
                    userPrefs[datum.UserID][datum.LocationID].Add(datum.Rating);
                }
            }

            // Convert the List<double> to double[]
            var convertedUserPrefs = userPrefs.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToDictionary(
                    kvp2 => kvp2.Key,
                    kvp2 => kvp2.Value.ToArray()
                )
            );

            // Convert double[] to double by taking the average
            var averagedUserPrefs = convertedUserPrefs.ToDictionary(
                kvp => kvp.Key,
                kvp => kvp.Value.ToDictionary(
                    kvp2 => kvp2.Key,
                    kvp2 => kvp2.Value.Average()
                )
            );

            return averagedUserPrefs;
        }

        private Matrix<double> CreateUserItemMatrix(Dictionary<int, double> userPrefs, List<int> otherUsers)
        {
            int maxItemId = Math.Max(userPrefs.Keys.Max(), data.Max(d => d.LocationID)) + 1;
            int numUsers = otherUsers.Count;

            Console.WriteLine($"Creating ratings matrix with {numUsers} users and {maxItemId} items...");

            var ratingsMatrix = new SparseMatrix(numUsers, maxItemId);

            for (int i = 0; i < numUsers; i++)
            {
                Console.WriteLine($"Processing user {otherUsers[i]} ({i + 1}/{numUsers})...");

                var preferences = GetUserPreferences(otherUsers[i]);
                foreach (var preference in preferences)
                {
                    int itemId = preference.Key;
                    if (itemId < maxItemId)
                        ratingsMatrix[i, itemId] = preference.Value;
                }
            }

            Console.WriteLine("Ratings matrix created successfully.");

            return ratingsMatrix;
        }

        private Dictionary<int, double> CalculateUserSimilarities(Matrix<double> ratingsMatrix, int userId)
        {
            var userPrefs = GetUserPreferences(userId);
            var otherUsers = GetOtherUsers(userId);

            var similarities = new Dictionary<int, double>();

            for (int i = 0; i < ratingsMatrix.RowCount; i++)
            {
                var preferences = GetUserPreferences(otherUsers[i]);
                var similarity = CalculateUserSimilarity(userPrefs, preferences);
                similarities[otherUsers[i]] = similarity;
            }

            return similarities;
        }

        private double CalculateUserSimilarity(Dictionary<int, double> prefs1, Dictionary<int, double> prefs2)
        {
            if (prefs1.Count == 0 || prefs2.Count == 0)
                return 0;

            var vector1 = DenseVector.Build.Dense(prefs1.Values.ToArray());
            var vector2 = DenseVector.Build.Dense(prefs2.Values.ToArray());

            // Adjust the dimensions of the vectors to match
            var maxDimension = Math.Max(vector1.Count, vector2.Count);
            if (vector1.Count < maxDimension)
                vector1 = DenseVector.OfEnumerable(vector1.Concat(new double[maxDimension - vector1.Count]));
            else if (vector2.Count < maxDimension)
                vector2 = DenseVector.OfEnumerable(vector2.Concat(new double[maxDimension - vector2.Count]));

            double dotProduct = vector1.DotProduct(vector2);
            double magnitude1 = vector1.L2Norm();
            double magnitude2 = vector2.L2Norm();

            if (magnitude1 == 0 || magnitude2 == 0)
                return 0;

            return dotProduct / (magnitude1 * magnitude2);
        }
    }
}
