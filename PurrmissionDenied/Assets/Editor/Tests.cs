using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

public static class Tests
{
#if UNITY_EDITOR

    [MenuItem("Validate/DayDefinitions")]
    public static void TestDayDefinitions()
    {
        Regex dateRegex = new Regex(@"^[0-9]{4}-[0-9]{2}-[0-9]{2}$");
        RunTest<DayDefinition>(
            (item, reporter) =>
            {
                AssertNotNull(item.Index);
                AssertNotNull(item.Date);
                AssertNotNull(item.Directory);
                AssertNotNull(item.PoolDefinitions);
                AssertNotNull(item.PoolOrder);

                if (item.Directory == "")
                    throw new Exception("Assertion failed, directory is empty");
                if (item.Index < 1)
                    throw new Exception("Assertion failed, day indices count up from 1");
                if (!dateRegex.IsMatch(item.Date))
                    throw new Exception(
                        "Assertion failed, day date does not match pattern YYYY-MM-DD"
                    );
                if (item.PoolDefinitions.Count() == 0)
                    throw new Exception("Assertion failed, a pool definition must exist");
                if (item.PoolOrder.Count() == 0)
                    throw new Exception("Assertion failed, a pool order must exist");

                // Check pool definitions and orders
                Dictionary<int, int> pulls = new Dictionary<int, int>();
                List<string> files = new List<string>();

                foreach (var (def, idx) in item.PoolDefinitions.Select((def, idx) => (def, idx)))
                {
                    if (def.UserFiles.Count() == 0)
                        throw new Exception(
                            "Assertion failed, all user pool definitions must contain at least one file"
                        );
                    foreach (string file in def.UserFiles)
                    {
                        if (files.FindAll(filename => filename == file).Count() > 0)
                            throw new Exception(
                                $"Assertion failed, duplicate dm filename '{file}' present multiple times"
                            );
                        files.Add(file);
                    }
                    pulls.Add(idx, def.UserFiles.Count());
                }
                HashSet<int> unusedIndices = new HashSet<int>(
                    Enumerable.Range(0, item.PoolDefinitions.Count())
                );
                foreach (UserPool pool in item.PoolOrder)
                {
                    if (pool.PoolIndex < 0 || pool.PoolIndex >= item.PoolDefinitions.Count())
                        throw new Exception(
                            $"Assertion failed, pool order pool index must be in range {0} to {item.PoolDefinitions.Count() - 1}"
                        );
                    if (pool.UserCount < 1)
                        throw new Exception(
                            $"Assertion failed, pool order user count must be at least 1"
                        );
                    unusedIndices.Remove(pool.PoolIndex);
                    if (pool.UserCount > pulls[pool.PoolIndex])
                        throw new Exception(
                            $"Assertion failed, pool order requested more users than are present in definition {pool.PoolIndex}"
                        );
                    pulls[pool.PoolIndex] -= pool.UserCount;
                }
                if (unusedIndices.Count() > 0)
                    throw new Exception(
                        $"Assertion failed, not all pools are used by orders. Unused indices: {String.Join(", ", unusedIndices)}"
                    );

                foreach (var pair in pulls)
                {
                    if (pair.Value > 0)
                        reporter.Warn(
                            $"There are unused user files in user pool {pair.Key}. If this is intentional, ignore this warning."
                        );
                }

                // Load All Files from PoolDefinitions
                List<UserEntry> entries = ImportDir<UserEntry>(
                    Path.Combine("lang", "en", "days", item.Directory),
                    files
                );

                // Verify file validity
                foreach (UserEntry entry in entries)
                {
                    AssertNotNull(entry.appeal_message);
                    AssertNotNull(entry.bio);
                    AssertNotNull(entry.date);
                    AssertNotNull(entry.image_index);
                    AssertNotNull(entry.messages);
                    AssertNotNull(entry.name);

                    if (!dateRegex.IsMatch(entry.date))
                        throw new Exception(
                            "Assertion failed, user entry date does not match pattern YYYY-MM-DD"
                        );
                    if (entry.image_index == 0)
                        reporter.Warn("A user entry's image_index is 0. Is that a mistake?");

                    foreach (string message in entry.messages)
                    {
                        if (!(message.Count() > 0))
                            reporter.Warn("A user entry has an empty message. Is that a mistake?");
                    }

                    if (entry.name.Count() == 0)
                        reporter.Warn("A user entry's name is empty. Is that a mistake?");
                }
            }
        );
    }

    [MenuItem("Validate/DirectMessagePoolDefinitions")]
    public static void TestDirectMessagePoolDefinitions()
    {
        RunTest<DirectMessagePoolDefinition>(
            (item, reporter) =>
            {
                AssertNotNull(item.directory);
                AssertNotNull(item.dmFiles);

                if (item.directory == "")
                    throw new Exception("Assertion failed, directory is empty");
                if (item.dmFiles.Count() == 0)
                    throw new Exception("Assertion failed, dmFiles length is 0");

                TestDMsInDir(Path.Combine("lang", "en", "messages", item.directory), item.dmFiles);
            }
        );
    }

    [MenuItem("Validate/DirectMessageSequenceDefinitions")]
    public static void TestDirectMessageSequenceDefinitions()
    {
        RunTest<DirectMessageSequenceDefinition>(
            (item, reporter) =>
            {
                AssertNotNull(item.directory);
                AssertNotNull(item.dmFiles);

                if (item.directory == "")
                    throw new Exception("Assertion failed, directory is empty");
                if (item.dmFiles.Count() == 0)
                    throw new Exception("Assertion failed, dmFiles length is 0");

                TestDMsInDir(Path.Combine("lang", "en", "messages", item.directory), item.dmFiles);
            }
        );
    }

    private static void TestDMsInDir(string dir, List<string> dmFiles)
    {
        foreach (string filename in dmFiles)
        {
            int presence = dmFiles.FindAll(item => item == filename).Count();
            if (presence > 1)
                throw new Exception(
                    $"Assertion failed, duplicate dm filename '{filename}' present {presence} times"
                );
        }

        List<DirectMessage> messages = ImportDir<DirectMessage>(dir, dmFiles);
        foreach (DirectMessage message in messages)
        {
            AssertNotNull(message.message);
        }
    }

    private static void AssertNotNull<T>(T nullable)
    {
        if (nullable == null)
            throw new Exception("Assertion failed, value is null");
    }

    private static List<T> ImportDir<T>(string dir, List<string> files)
    {
        string path = Path.Combine(Application.streamingAssetsPath, dir);
        List<T> items = new List<T>();
        foreach (string filename in files)
        {
            items.Add(ImportFile<T>(Path.ChangeExtension(Path.Combine(path, filename), "json")));
        }
        return items;
    }

    private static T ImportFile<T>(string filename)
    {
        string contents = ReadFileToString(filename);
        return JsonUtility.FromJson<T>(contents);
    }

    private static string ReadFileToString(string path)
    {
        using (StreamReader reader = new StreamReader(path))
        {
            return reader.ReadToEnd();
        }
    }

    private const string CHARS = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
    private const int RUN_ID_LENGTH = 6;

    private static void RunTest<T>(Action<T, Reporter> test)
        where T : UnityEngine.Object
    {
        System.Random rand = new System.Random();
        List<char> chars = new List<char>();
        for (int idx = 0; idx < RUN_ID_LENGTH; idx++)
        {
            chars.Add(CHARS[rand.Next(CHARS.Length)]);
        }
        string runID = String.Join("", chars);

        T[] items = GetAll<T>().ToArray();
        int total = items.Count();
        Debug.Log($"{runID} - Validating {total} {typeof(T).Name}(s)...");
        List<(Reporter, Exception)> results = new List<(Reporter, Exception)>();

        foreach (var item in items)
        {
            Reporter reporter = new Reporter();
            Exception err = null;
            try
            {
                test(item, reporter);
            }
            catch (Exception caughtErr)
            {
                err = new Exception($"Error validating {typeof(T).Name}", caughtErr);
            }
            results.Add((reporter, err));
        }

        foreach (var ((reporter, fail), idx) in results.Select((item, idx) => (item, idx)))
        {
            foreach (string info in reporter.Infos)
            {
                Debug.Log($"{runID} - Test {idx} - {info}");
            }
            foreach (string warning in reporter.Warnings)
            {
                Debug.LogWarning($"{runID} - Test {idx} - {warning}");
            }
            if (fail != null)
            {
                string msg = fail.Message;
                Exception err = fail.InnerException;
                while (err != null)
                {
                    msg = msg + "\n" + err.Message;
                    err = err.InnerException;
                }
                Debug.LogError($"{runID} - Test {idx} - {msg}");
            }
        }

        int failCount = results
            .Where(((Reporter reporter, Exception err) result) => result.err != null)
            .Select(((Reporter reporter, Exception err) result) => (result.reporter, result.err))
            .Count();

        if (failCount > 0)
        {
            Debug.LogError($"{runID} - {failCount}/{total} {typeof(T).Name}(s) failed to validate");
        }
        else
        {
            Debug.Log($"{runID} - All {total} {typeof(T).Name}(s) validated successfully");
        }
    }

    class Reporter
    {
        private List<string> warnings = new List<string>();
        private List<string> infos = new List<string>();

        public IEnumerable<string> Warnings
        {
            get => warnings;
        }
        public IEnumerable<string> Infos
        {
            get => infos;
        }

        public void Warn(string msg)
        {
            warnings.Add(msg);
        }

        public void Info(string msg)
        {
            infos.Add(msg);
        }
    }

    private static IEnumerable<T> GetAll<T>()
        where T : UnityEngine.Object
    {
        string[] guids = AssetDatabase.FindAssets($"t:{typeof(T).Name}");
        return guids
            .Select(guid => AssetDatabase.GUIDToAssetPath(guid))
            .Select(path => AssetDatabase.LoadAssetAtPath<T>(path))
            .Where(item => item != null);
    }
#endif
}
