1.
Question 1
What is the goal of regression testing?
To check whether new functionality works correctlyTo determine whether previously developed software performs the same way after modificationsTo add to test suites to more rigorously test existing softwareTo determine whether or not a bug in a previous version of the software is fixedThe first and second answers are trueAll of the above

1 point
2.
Question 2
Why is regression testing well-suited for automated verification?
The tests are low cost to generate so it is inexpensive to throw tests away.It is straightforward to generate many tests using automated verification.A good oracle for determining success of automated tests is always available.

1 point
3.
Question 3
What is the goal of fuzz testing?
To check whether software has good real-time performanceTo check whether software is robust (does not crash)To check whether software has security bugsTo check whether software meets its functional requirements

1 point
4.
Question 4
How is smart grammar-based fuzz testing different than adaptive random testing?
Grammar-based fuzzing is more likely to generate more almost-correct malformed inputs than adaptive random testingThey are not substantially differentAdaptive random testing uses more sophisticated metrics for generating the next testGrammar-based fuzzing has more knowledge of valid inputs

1 point
5.
Question 5
Fuzz security testing is only concerned about generating malformed inputs.
FalseTrue

1 point
6.
Question 6
What is the main source of human effort for grammar-based fuzzing?
Writing testsManually instrumenting the programCreating the grammar to be used by the fuzzerDetermining what kind of fault was demonstrated by the testAll of the above require similar levels of human effort

1 point
7.
Question 7
What steps can be used to improve fuzzing performance?
Biasing inputs for random testingRunning multiple fuzzers concurrentlyRunning each fuzzer for longer periods of timeAdding grammar information to the fuzzerAll of the above

1 point
8.
Question 8
Are fuzzers optimistic or pessimistic verification tools?
Optimistic - They may miss existing security problems in programsPessimistic - They only find crashes or security flaws.

1 point
9.
Question 9
Which of the following can be performed with runtime monitoring?
Measuring whether application real-time performance is adequateMonitoring whether the application meets its functional requirementsDetermining whether the program is correctMeasuring whether the environment matches program assumptionsDetermining whether the program will terminate

1 point
10.
Question 10
What is a "fail-safe" system?
One in which the software or physical systems can fail leaving the system in a safe state.One in which the software makes the system safe in case of physical failures.One in which there is redundancy so any single physical or computer failure will not cause the system to fail.All of the above

1 point
11.
Question 11
If software is not fail-safe, is there any good reason for runtime monitoring?
No - in this case, the system will fail anyway.Yes - we can give control back to the operator so that they can control a process manually.Yes - we can provide warnings to the operator that the software may not be behaving correctly.Yes - this technique lets the software recover from failures and keep going.No - it slows the program down too much.

1 point
12.
Question 12
Automated testing should replace writing test cases by hand.
TrueFalse

1 point
13.
Question 13
The most useful automated verification technique that we have examined is
Random testingAdaptive random testingFuzz testingSearch-based testingStatic analysisIt depends.  What is most useful depends on the goal of testing.

1 point
14.
Question 14
None of the automated verification techniques scale to real programs.
TrueFalse

1 point
15.
Question 15
Knowing one testing technique is enough to be an effective test engineer.
TrueFalse

1 point
16.
Question 16
The main problems with random testing are:
it does not scale to real programsit is unable to explore paths that rely on specific inputs given a large input spaceinput distributions in practice may not match random distributions used during testing, so the likelihood of failure may be larger than expected in actual use.it takes a lot of time to generate each test

1 point
17.
Question 17
The main problems with symbolic execution are:
dealing with program paths containing non-linear mathhandling complex string operationschoosing the correct value that solves complex linear constraintstoo many paths in large programsprogram dependencies (like databases) that are unknown to the symbolic solver

1 point
18.
Question 18
Successful automated testing always requires
a machine-checkable oracle that can determine success or failure.access to the internals of the program to monitor program state.access to really fast computers to generate the tests.All of the above

1 point
19.
Question 19
Choose which of the following are true:
Test automation is not recognized as a useful skill in the industry.Testers will also be expected to develop software in the future.According to a recent survey, more than 1/2 of businesses are using test-generation tools.We understand how to test machine learning software like neural nets.Agile and DevOps are important skills for test engineers.

1 point
20.
Question 20
Which of the following describes the problem with testing machine learning systems?

No traditional program structure for test metricsDifficult to define requirements for program behaviorOnline evolution means that program behavior will changeAll of the above

1 point
21.
Question 21
Which of the following are true of systems of systems?
Parts evolve independentlyNo central governing authorityDecoupled execution allows separate testingInterfaces evolve over time