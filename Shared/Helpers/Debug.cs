using System;
using System.Runtime.CompilerServices;

namespace BDDReferenceService {

    /**
     * Debug helper class.
     */
    public static class Debug {

        public static void Uncoded([CallerMemberName] string memberName = "",
                                     [CallerFilePath] string sourceFilePath = "",
                                     [CallerLineNumber] int sourceLineNumber = 0) {
            //???LogHelper.AddLog(LogHelper.CHANNEL_DEBUG, $"Uncoded: {memberName} ({sourceFilePath}:{sourceLineNumber})");
        }

        public static void Untested([CallerMemberName] string memberName = "",
                                      [CallerFilePath] string sourceFilePath = "",
                                      [CallerLineNumber] int sourceLineNumber = 0) {
            //???LogHelper.AddLog(LogHelper.CHANNEL_DEBUG, $"Untested: {memberName} ({sourceFilePath}:{sourceLineNumber})");
        }

        public static void Unreachable([CallerMemberName] string memberName = "",
                                         [CallerFilePath] string sourceFilePath = "",
                                         [CallerLineNumber] int sourceLineNumber = 0) {
            //???LogHelper.AddLog(LogHelper.CHANNEL_DEBUG, $"Unreachable: {memberName} ({sourceFilePath}:{sourceLineNumber})");
        }

        public static void Tested() {
        }

        public static void Assert(bool value,
                                    [CallerMemberName] string memberName = "",
                                    [CallerFilePath] string sourceFilePath = "",
                                    [CallerLineNumber] int sourceLineNumber = 0) {
            if (!value) {
                //???LogHelper.AddLog(LogHelper.CHANNEL_DEBUG, $"Assertion: {memberName} ({sourceFilePath}:{sourceLineNumber})");
            }
        }

        public static void AssertNull(object obj,
                                        [CallerMemberName] string memberName = "",
                                        [CallerFilePath] string sourceFilePath = "",
                                        [CallerLineNumber] int sourceLineNumber = 0) {
            Assert((obj == null), memberName, sourceFilePath, sourceLineNumber);
        }

        public static void AssertValid(object obj,
                                         [CallerMemberName] string memberName = "",
                                         [CallerFilePath] string sourceFilePath = "",
                                         [CallerLineNumber] int sourceLineNumber = 0) {
            Assert((obj != null), memberName, sourceFilePath, sourceLineNumber);
        }

        public static void AssertValidOrNull(object obj,
                                               [CallerMemberName] string memberName = "",
                                               [CallerFilePath] string sourceFilePath = "",
                                               [CallerLineNumber] int sourceLineNumber = 0) {
            if (obj != null) {
                AssertValid(obj, memberName, sourceFilePath, sourceLineNumber);
            }
        }

        public static void AssertString(string str,
                                          [CallerMemberName] string memberName = "",
                                          [CallerFilePath] string sourceFilePath = "",
                                          [CallerLineNumber] int sourceLineNumber = 0) {
            Assert((str != null), memberName, sourceFilePath, sourceLineNumber);
            if (str != null) {
                Assert((str.Length > 0), memberName, sourceFilePath, sourceLineNumber);
            }
        }

        public static void AssertStringOrNull(string str,
                                                [CallerMemberName] string memberName = "",
                                                [CallerFilePath] string sourceFilePath = "",
                                                [CallerLineNumber] int sourceLineNumber = 0) {
            if (str != null) {
                AssertString(str, memberName, sourceFilePath, sourceLineNumber);
            }
        }

        public static void AssertID(string ID,
                                      [CallerMemberName] string memberName = "",
                                      [CallerFilePath] string sourceFilePath = "",
                                      [CallerLineNumber] int sourceLineNumber = 0) {
            Assert(Helper.IsValidID(ID), memberName, sourceFilePath, sourceLineNumber);
        }

        //??? public static void AssertIDOrZero(string ID,
        //                                     [CallerMemberName] string memberName = "",
        //                                     [CallerFilePath] string sourceFilePath = "",
        //                                     [CallerLineNumber] int sourceLineNumber = 0)
        // {
        //     if (ID != 0)
        //     {
        //         AssertID(ID, memberName, sourceFilePath, sourceLineNumber);
        //     }
        // }

        public static void AssertIDOrNull(string ID,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0) {
            if (ID != null) {
                AssertID(ID, memberName, sourceFilePath, sourceLineNumber);
            }
        }

        public static void AssertEmail(string email,
                                         [CallerMemberName] string memberName = "",
                                         [CallerFilePath] string sourceFilePath = "",
                                         [CallerLineNumber] int sourceLineNumber = 0) {
            Assert(Helper.IsValidEmail(email), memberName, sourceFilePath, sourceLineNumber);
        }

        public static void AssertEmailOrNull(string email,
                                               [CallerMemberName] string memberName = "",
                                               [CallerFilePath] string sourceFilePath = "",
                                               [CallerLineNumber] int sourceLineNumber = 0) {
            if (email != null) {
                Assert(Helper.IsValidEmail(email), memberName, sourceFilePath, sourceLineNumber);
            }
        }

        public static void AssertPassword(string password,
                                            [CallerMemberName] string memberName = "",
                                            [CallerFilePath] string sourceFilePath = "",
                                            [CallerLineNumber] int sourceLineNumber = 0) {
           Assert(Helper.IsValidPassword(password, false), memberName, sourceFilePath, sourceLineNumber);
        }

    }   // Debug

}   // BDDShared
