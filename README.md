# ItineraryGenerator
Travelling salesperson problem using a heuristic algorithm. 

Command line program accepts a mail file, plane specfiication file, trip starting time and output file. The program produces an intinerary for the flight listing the order in which stations should be visited so as to minimize the tour length.

## Example input: 

flying-postman.exe mail.txtboeing-spec.txt23:00 –o itinerary.txt

## Example output:

Reading input from mail.txt\
Optimising tour length: Level 1...\
Elapsed time: 123 seconds.\
Tour time: 7 Hours 24 Minutes.\
Tour length: 972.5855\
qaz      ->      yhn     23:00   23:28\
yhn      ->      wsx     23:28   00:10\
wsx      ->      edc     00:10   00:44\
edc      ->      rfv     00:44   01:30\
*** refuel 10 minutes *** \
rfv      ->      tgb     01:40   02:14\
tgb      -> qaz     02:14   03:00\
Saving itinerary to itinerary.txt
