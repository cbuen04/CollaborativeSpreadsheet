# ps6-syntactic_sugar_dads
ps6-syntactic_sugar_dads created by GitHub Classroom
Read Me:

Oct. 13
On this day we did some prep work before ps6. We debugged the tests that the spreadsheet was failing. 
Fixed issues like the validator and XML reader.

Oct. 16
We set up the project, added the dll to have access to the grid and pasted the skeleton code into the solution. 
Placed the grid on the form.

Oct. 18
today we spent the beginning laying out the main UI we wanted to work with. 
Experienced several issues with the GUI breaking, we restarted a few times, 
and overall took time to get used to working with the form builder. 
Ended up adding text boxes to the spreadsheet, and allowed user input to the text boxes. 
Each box was also given their appropriate read only restrictions on all boxes that were not user input. 
The cursor also automatically focuses on the textbox to make a more seamless experience. 
Decided that there would be no buttons to allow the user to calculate rather just have the user hit enter. 
The button seemed redundant and hitting “enter” to place a cell value was more user intuitive as well as 
kept the look of the spreadsheet cleaner.

Oct. 19
The logic for the spreadsheet was implemented into the GUI. 
Users are now allowed to add formulas, numbers and strings, the visual graph is now backed by a structure. 
Added a new way to navigate the spreadsheet via arrow keys. New bug in code was found, adding a circular dependency 
on a new cell as the initial value broke the spreadsheet. The rest of the day was spent trying to debug this issue.

Oct. 20
We continued to attempt to debug the circular dependency for a little bit. 
Added to the UI adding toolbar that contained file creation, saving, and closing the window. 
We formatted and organized the options similar to excel, or google sheets. This was done in hopes that 
anyone who hasn’t used our spreadsheet before would be able to easily navigate and use it.

Logic for “new spreadsheet” was made and works, with the exception that the closing of the main window 
would close the whole app. More work needs to be done. Opening an existing file works but saving needs 
some work.

Both open and save took advantage of the file dialog classes and we used code from the microsoft 
documentation and modified it for our code.

Oct. 21
We were finally able to figure out the dependency bug in the code and get it fixed. 
Every exception and error is working as intended. The save feature was fixed and works as intended too. 
We decided to change the save button and give the user options to save or save as, like any other spreadsheet 
program does. This was an added functionality that allowed the user to save their file under the initially 
assigned name without needing them to type it in every time they wanted to save. This was done to make the 
user experience more seamless. A warning was also implemented to prevent users from closing the spreadsheet 
without saving, to prevent losing work. An option is given to save or leave without saving.

Labels were added to the spreadsheet to tell the user what information the text boxes were displaying. 
This was made to avoid confusion.

Oct. 22
On this day we focused on the main special features aside from the design choices made in the previous days. 
We added a features tab on the toolbar where all the functions made would reside. The main feature being a sum 
function. This function was interesting to implement, requiring us to edit the UI to change when the user 
activated the feature, to accommodate the functionality needed. We added two new buttons that only appear when 
the sum feature is activated. The user could then select all the cells they want to add together and the cell 
outputs that value. This was done to make it easier than having to manually type all the cell names.

The second feature was added which when activated, changed the UI appearance to pink. 
This was to explore customization as well as possibility for accessibility functionalities like color 
inversion, or darkmodes that would be easier on the eyes.

We also added a new section to the toolbar dedicated to loading the help information. 
We were able to do this by referencing stack overflow, concerning how to load a text file to a text box. 
We used File.ReadAllText() and loaded that into the text box.

Finally we tested our spreadsheet again, and typed up our help text file for the program and finished 
the program. Everything was committed to github.
