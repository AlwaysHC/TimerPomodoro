using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

namespace TimerPomodoro;

internal sealed record PomodoroState(int Total, int Today, int Yesterday, DateOnly Date) {
    public PomodoroState AdvanceTo(DateOnly date) => date == Date
        ? this
        : this with { Today = 0, Yesterday = date.DayNumber - Date.DayNumber == 1 ? Today : 0, Date = date };
}

internal static class PomodoroStore {
    private const string FileName = "TimerPomodoro.txt";
    private static readonly string DirectoryPath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "TimerPomodoro");
    internal static string FilePath => Path.Combine(DirectoryPath, FileName);

    internal static (PomodoroState State, bool NeedsSave) Load() {
        string[] paths = [FilePath, Path.GetFullPath(FileName), Path.Combine(AppContext.BaseDirectory, FileName)];
        foreach (string path in paths.Distinct(StringComparer.OrdinalIgnoreCase)) {
            string[] lines;
            try {
                lines = File.ReadAllLines(path);
            }
            catch (FileNotFoundException) {
                continue;
            }
            catch (DirectoryNotFoundException) {
                continue;
            }

            if (lines.Length != 4
                || !int.TryParse(lines[0], NumberStyles.None, CultureInfo.InvariantCulture, out int total)
                || !int.TryParse(lines[1], NumberStyles.None, CultureInfo.InvariantCulture, out int today)
                || !int.TryParse(lines[2], NumberStyles.None, CultureInfo.InvariantCulture, out int yesterday)
                || (long)today + yesterday > total
                || !TryReadDate(lines[3], out DateOnly date)) {
                throw new FormatException($"The saved counters in '{path}' are invalid. The file has not been changed.");
            }

            return (new(total, today, yesterday, date), !StringComparer.OrdinalIgnoreCase.Equals(path, FilePath));
        }

        return (new(0, 0, 0, DateOnly.FromDateTime(DateTime.Today)), false);
    }

    private static bool TryReadDate(string text, out DateOnly date) {
        if (DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)) {
            return true;
        }

        if (DateTime.TryParse(text, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime legacyDate)) {
            date = DateOnly.FromDateTime(legacyDate);
            return true;
        }

        return false;
    }

    internal static void Save(PomodoroState state) {
        Directory.CreateDirectory(DirectoryPath);
        string temporaryPath = $"{FilePath}.{Guid.NewGuid():N}.tmp";
        using (FileStream stream = new(temporaryPath, FileMode.CreateNew, FileAccess.Write, FileShare.None)) {
            using StreamWriter writer = new(stream, new UTF8Encoding(false), leaveOpen: true);
            writer.WriteLine(state.Total.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine(state.Today.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine(state.Yesterday.ToString(CultureInfo.InvariantCulture));
            writer.WriteLine(state.Date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture));
            writer.Flush();
            stream.Flush(true);
        }

        if (File.Exists(FilePath)) {
            File.Replace(temporaryPath, FilePath, null);
        }
        else {
            File.Move(temporaryPath, FilePath);
        }
    }
}
