using Microservice.Models.Matches;
using Microservice.Models.Series;

namespace Microservice.Test.Unit;

[TestClass]
public class SetTests
{
  [TestMethod]
  public void Scores_NoMatches()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Set set = new("set", 3, new[] { "team1", "team2" }, seeds);

    // Act
    Dictionary<string, int> scores = set.Scores;

    // Assert
    Assert.AreEqual(0, scores["team1"]);
    Assert.AreEqual(0, scores["team2"]);
  }

  [TestMethod]
  public void Scores_MidSet()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Dictionary<string, int> scores1 = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };
    Dictionary<string, int> scores2 = new()
    {
      { "team1", 2 },
      { "team2", 1 }
    };
    Dictionary<string, int> scores3 = new()
    {
      { "team1", 3 },
      { "team2", 0 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores1, seeds) },
      { "match2", new("match2", scores2, seeds) },
      { "match3", new("match3", scores3, seeds) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    Dictionary<string, int> scores = set.Scores;

    // Assert
    Assert.AreEqual(2, scores["team1"]);
    Assert.AreEqual(1, scores["team2"]);
  }

  [TestMethod]
  public void Scores_Forfeited()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Set set = new("set", 3, new[] { "team1", "team2" }, seeds);

    // Act
    set.Forfeit("team1");

    // Assert
    Assert.AreEqual(0, set.Scores["team2"]);
    Assert.AreEqual(-1, set.Scores["team1"]);
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_TwoTeams()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Dictionary<string, int> scores = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores, seeds) },
      { "match2", new("match2", scores, seeds) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    string[] standings = set.Standings;

    // Assert
    Assert.AreEqual("team2", standings[0]);
    Assert.AreEqual("team1", standings[1]);
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_FourTeams()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2", "team3", "team4" };
    Dictionary<string, int> scores1 = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };
    Dictionary<string, int> scores2 = new()
    {
      { "team3", 3 },
      { "team4", 4 }
    };
    Dictionary<string, int> scores3 = new()
    {
      { "team2", 5 },
      { "team4", 6 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores1, seeds) },
      { "match2", new("match2", scores2, seeds) },
      { "match3", new("match3", scores3, seeds) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    string[] standings = set.Standings;

    // Assert
    Assert.AreEqual("team4", standings[0]);
    Assert.AreEqual("team2", standings[1]);

    // TODO: Implement seeding to check positions of [2] and [3] with team1 and team3
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_TiedScore()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Dictionary<string, int> scores1 = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };
    Dictionary<string, int> scores2 = new()
    {
      { "team1", 2 },
      { "team2", 1 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores1, seeds, true, true) },
      { "match2", new("match2", scores2, seeds, true, true) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    string[] standings = set.Standings;

    // Assert
    Assert.AreEqual("team1", standings[0]);
    Assert.AreEqual("team2", standings[1]);
  }

  [TestMethod]
  public void Standings_IsCorrectOrder_TiedScore_AlternateSeeding()
  {
    // Arrange
    string[] seeds = new[] { "team2", "team1" };
    Dictionary<string, int> scores1 = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };
    Dictionary<string, int> scores2 = new()
    {
      { "team1", 2 },
      { "team2", 1 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores1, seeds, true, true) },
      { "match2", new("match2", scores2, seeds, true, true) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    string[] standings = set.Standings;

    // Assert
    Assert.AreEqual("team2", standings[0]);
    Assert.AreEqual("team1", standings[1]);
  }

  [TestMethod]
  public void State_NotStarted_NoMatch()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Set set = new("set", 3, new[] { "team1", "team2" }, seeds);

    // Act
    SetState state = set.State;

    // Assert
    Assert.AreEqual(SetState.NotStarted, state);
  }

  [TestMethod]
  public void State_NotStarted_PreMatch()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", new[] { "team1", "team2" }, seeds) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    SetState state = set.State;

    // Assert
    Assert.AreEqual(SetState.NotStarted, state);
  }

  [TestMethod]
  public void State_InProgress_FirstMatch()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Dictionary<string, int> scores = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores, seeds) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    SetState state = set.State;

    // Assert
    Assert.AreEqual(SetState.InProgress, state);
  }

  [TestMethod]
  public void State_InProgress_PreSecondMatch()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Match match1 = new("match1", new Dictionary<string, int>()
    {
      { "team1", 1 },
      { "team2", 2 }
    }, seeds);
    match1.Finish();
    
    Dictionary<string, Match> matches = new()
    {
      { "match1", match1 },
      { "match2", new("match2", new[] { "team1", "team2" }, seeds) }
    };

    Set set = new("set", 3, matches, seeds);

    // Act
    SetState state = set.State;

    // Assert
    Assert.AreEqual(SetState.InProgress, state);
  }

  [TestMethod]
  public void State_Completed_Normal()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Dictionary<string, int> scores1 = new()
    {
      { "team1", 1 },
      { "team2", 2 }
    };
    Dictionary<string, int> scores2 = new()
    {
      { "team1", 0 },
      { "team2", 3 }
    };

    Dictionary<string, Match> matches = new()
    {
      { "match1", new("match1", scores1, seeds) },
      { "match2", new("match2", scores2, seeds) }
    };

    Set set = new("set", 2, matches, seeds);

    // Act
    SetState state = set.State;

    // Assert
    Assert.AreEqual(SetState.Completed, state);
  }

  [TestMethod]
  public void State_Completed_Forfeited()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Set set = new("set", 3, new[] { "team1", "team2" }, seeds);

    // Act
    set.Forfeit("team1");

    // Assert
    Assert.AreEqual(SetState.Completed, set.State);
  }

  [TestMethod]
  public void Set_CreatesNew()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    string[] teams = new[] { "team1", "team2" };
    
    // Act
    Set set = new("set", 3, teams, seeds);

    // Assert
    Assert.AreEqual(SetState.NotStarted, set.State);
    Assert.AreEqual(2, set.Teams.Length);
    Assert.AreEqual(0, set.Matches.Count);
  }

  [TestMethod]
  public void Set_CreatesExisting()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Match match = new("match", new Dictionary<string, int>()
    {
      { "team1", 1 },
      { "team2", 2 }
    }, seeds);

    Dictionary<string, Match> matches = new()
    {
      { "match", match }
    };

    // Act
    Set set = new("set", 3, matches, seeds);

    // Assert
    Assert.AreEqual(SetState.InProgress, set.State);
    Assert.AreEqual(2, set.Teams.Length);
    Assert.AreEqual(1, set.Matches.Count);
  }

  [TestMethod]
  public void Set_CreatesCompleted()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Match match1 = new("match1", new Dictionary<string, int>()
    {
      { "team1", 1 },
      { "team2", 2 }
    }, seeds);

    Match match2 = new("match2", new Dictionary<string, int>()
    {
      { "team1", 0 },
      { "team2", 3 }
    }, seeds);

    Dictionary<string, Match> matches = new()
    {
      { "match1", match1 },
      { "match2", match2 }
    };

    // Act
    Set set = new("set", 2, matches, seeds);

    // Assert
    Assert.AreEqual(SetState.Completed, set.State);
    Assert.AreEqual(2, set.Teams.Length);
    Assert.AreEqual(2, set.Matches.Count);
  }

  [TestMethod]
  public void SetGoal_UpdatesGoal()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Set set = new("set", 3, new[] { "team1", "team2" }, seeds);

    // Act
    set.SetGoal(5);

    // Assert
    Assert.AreEqual(5, set.Goal);
  }

  [TestMethod]
  public void Forfeit_ForfeitsTeam()
  {
    // Arrange
    string[] seeds = new[] { "team1", "team2" };
    Set set = new("set", 3, new[] { "team1", "team2" }, seeds);

    // Act
    set.Forfeit("team1");

    // Assert
    Assert.IsTrue(set.Forfeits.Contains("team1"));
  }
}