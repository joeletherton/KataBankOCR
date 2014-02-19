KataBankOCR
===========

This is a demo project that was specifically requested. The goal of this particular kata is to complete a text analysis process using TDD to accomplish the entire project. The original kata can be found on CodeDojo: http://codingdojo.org/cgi-bin/index.pl?KataBankOCR. My instructions were to complete User Stories 1 and 2, but I pseudo-completed User Story 3 as well. By pseudo-completed I mean I did not perform the actual save of a file, but I did output the results in the requested manner to the screen.

I figured since .Net is my space, it would be fairly simple to build a dirty Winforms application to do this. I probably could have saved even more time going with a console, but I dislike console applications. It's a personal bias unrelated to any particular technological reason.

So I started with a Winforms project and added a Microsoft Test project to go with it. The first thing I did, just to keep it clean, was delete the automatically generated initial test nonsense. Then I added the KataBankOCRTest class to hold my tests. Then I sat down and thought about the problem itself.

Obviously it was going to be reading text, but the keys were going to be analyzing the text patterns to guarantee that we got valid characters. So right away I knew I would need some basic methods, but I left the names and signature a little open:

    GetSlice - To get a particular slice in the array and verify it
    GetCharacter - To get a collection of slices in a coherent character
    GetAccountNumber - To get a collection of characters in a coherent account number
    
Since I was asked to do User Story 2 as well, I knew I would need a checksum method. It was after reading that requirement that I decided doing User Story 3 would be almost a no brainer since I was already planning to perform display in a window. So I decided on my checksum method:

    IsValidChecksum - Original right? I thought so too.
    
So I stubbed these methods out and started expanding my tests. I created tests for each character where necessary, and I took from the kata page that there would need to be some validations. I put a little thought into it, and while there are probably a lot of ways to retrieve a recognizable character, I felt it would be a nifty idea to use the unix method of masking (as observed in chmod) to identify numeric combinations uniquely.

So I drew a grid from [0,0] -> [2,2] left to right, top to bottom. I logically assigned each a numerical value starting with 0 in the upper left corner to 8 in the lower right corner. Then operating on a 2^n formula each cell is given a final numeric value -> 1, 2, 4, 8, 16, 32, 64, 128, 256. The purpose of this is that when slices of the character are added by their numerical values, each one would have a unique "mask" that could be applied. Restricting the operations to a mathematical / boolean comparison rather than a larger text comparison would hopefully speed the process up a bit.

So that made the test for GetSlice a little easier. It became GetSliceMask, and it simply returned a numeric. So testing was fairly simple. From there, I knew that getting the character was going to be fairly similar. It was also going to change to GetCharacterMask and also return a numeric. However, since the account number was a string value, a translation was going to be necessary at some point so GetCharacter made a comeback, but it remained simple. I decided by establishing an enum to match the mask values, the character generation became a switch on the mask. 

Once the individual characters were complete it came to constructing the account number itself. Since the rules indicate the characters are in 4 rows of 27 characters, I added some tests for line validation methods and proceeded to code them up. I also decided I would need some test methods to make sure that bad characters could be validated so I added those. This revealed a small bug in some of the math which I corrected. It also uncovered a potential of exception if the lines were not validated for length, so I added a test for this metric and a method to compensate for it.

Finally, having the account number completed, it was time to code up the IsValidChecksum. The logic wasn't particular tricky, but it's not necessarily intuitive so I could see how someone could get it all backwards. I took a couple of the test cases from the kata definition, and since it's a boolean function the test was easy to wire up. A little code later, my cases were working.

Once all of this was complete, it was time to integrate it into my application with a BackgroundWorker. I added my forms controls, my test file, and wired up the form elements. Because I'd set up my test cases to handle lines as if they'd been pulled from a file, I had no trouble with any of the tested functionality. All of my debugging led to things that were local to the presentation application.

Now having typed this up, I've realized there is precious little documentation in the code itself so it's time to go back and document a little more.
