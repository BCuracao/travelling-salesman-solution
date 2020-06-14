# msgCodeChallenge
My entry for the msg get_in_it challenge

HOW TO RUN:

1. Download and unzip the project
2. Open Project using VISUAL STUDIO
3. Run Solution
4. Result should appear in a console window

Algorithm used: Nearest Neighbour

HOW IT WORKS:
1. Read the .CSV file and store it as string array.
2. Left shift the array to remove the first line, which we don't need.
3. Transform the data to a XML format for easier processing of the data.
4. Extract the destination name and coordinates. We dont need anything else.
5. Assuming that you can travel from any city to any other; we have to calculate all possible distances between every single city.
6. Store the distance information in a 2d matrix (21x21)
7. Solve the Traveling Salesman Problem using the "Nearest Neighbour" approach.
