# Implementation

## Milestones

Date | Item | Signoffs
---- | ---- | --------
November 25, 2015 | Discord Meeting
November 27, 2015 | Manditory Class Attendance
November 28, 2015 | Half of assigned controllers completed
December 2, 2015  | Assigned controllers completed (with views) 
December 9, 2015  | Unit Tests for Assigned controllers completed
December 9, 2015  | Technical & Client Documentation
December 9, 2015  | Installer CD for server
December 11, 2015 | Final Due Date

## Work Items List
- ~~Create the solution~~
- Code First Database *started*
  - [Mock Data](http://www.generatedata.com/) *started*
    -Friendship(mutual) data
    -Games data
    -Reviews data
    -Wishlist data (maybe?)
    -Sales data (for sales report)
  - Get / Create Game Images and store in db as path
- ~~Build db from code first classes~~
- ~~Add controllers as per design class digram~~
  - Unit Tests
  - Minimal Views (just functional) *started*
- ~~Add partial classes for validation ~~
- Doc comments on methods / functions *started*
- GUI Design See Nicole *started*
- User Auth See John
  - ~~Sign up: require a unique display name, require strong passwords~~
  - ~~Log in: limit the number of consecutive login attempts~~
  - ~~Find out how long user is locked out for after 5 consecutive attempts~~
  - Password: let members change their passwords; if a recognized member forgets their password; reset it and email the new password *started needs testing*
- ~~Roles See John~~
 - ~~Add layout views (header, footer) for each role (employee, member, visitor)~~
- ~~Revisit Events Table (Update ERD?)~~
- ~~Add String to games table for image path~~
- ~~ESRB Rating & Publisher add to DB~~
- Game controller
  - ~~let members search for games~~
  - ~~let members select games from a list~~
  - ~~let employees add, edit and delete games~~
  - ~~display details of the selected games~~
  - add to wishlist
  - display library of members ordered/downloaded games
  - ~~rate game: let 1) members rate games; 2) summarize individual results and display the overall rating for each game~~
  - ~~review games: let members write reviews of games.  Reviews must be approved by a moderator before they are published on the website~~
  - ~~review games: let employees view pending reviews for approval~~
  - ~~review games: let employees approve/disprove reviews~~
  - ~~review games: let members and visitors view reviews for a game~~
  - download: let members download free and shareware games
- User controller
  - Profile: let members enter their display name, actual name, email, gender and birth date; let the member decide whether to receive promotional emails from CVGS
    - **Started** 
    - fighting with modelstate
  - Preferences: let members indicate their: favourite platform(or platforms) and; favourite game category (or categories)
    - **Not Started**
    - requires db / model modification 
  - Address: let the member enter, modify and delete their 1) address; 2) "ship to" address
    - **Started** 
  - Credit Cards: let the member register one or more valid credit card Using Stripe
    - **Not Started**
- Friendship controller
  - ~~view wishlist (pass in id, either of friend or member's own id to view own wishlist).  Let only friends and relatives view the wish list with option~~
  - ~~Add member/members in the Friends and Family list~~
- Event controller
 - let employees add, edit and delete information about upcoming events.
 - let members register for upcoming events
- Order controller
  - ~~create a cart~~
  - ~~let members add games to cart~~
  - let members check out with ~~any credit card registered~~ Stripe
  - ~~save the "check out" info to a db table (orders) so that an employee can post the games physically~~
  - after posting, the employee should be able to mark that order as processed *started*
- Report controller
  - let employees view and print reports *started*
  - ~~game list report~~
  - ~~game detail report~~
  - ~~member list report~~
  - ~~member detail report~~
  - wish list report
  - sales report
