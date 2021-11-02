# City Planning Simulator

**Some notes about the function and description**

|       Class       |                       Description                             |
|       :----:      |                       :----                                   |
|   [PathFinder](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/PathFinder.cs)    | Creating vehicles and finding the shortest route              |
|   [Junction](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Other/Junction.cs)      | Object connecting streets with a traffic light or roundabout  |
|   [Street](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Other/Street.cs)          |Vehicle management on the road                                 |
|   [LoadData](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Scripts/LoadData.cs)        |Load data from *txt file                                       |
|   [PathFollower](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Other/PathFollower.cs)    |Set the direction of movement of vehicles                      |
|   [GraphData](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Other/GraphData.cs)       |A collection of nodes, paths and objects link to them          |
|   [Node](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Other/Node.cs)            | Single Node. From which will be created Paths                 |
|   [Path](https://github.com/MohitKumavat/NSF_City_Planning_Simulator/blob/main/Assets/Other/Path.cs)            |Path is a connection between 2 Nodes                           |

**Tasks to do in the project**
- [x] Integration of the vehicle location section into the map of Mohit
- [x] Creating random vehicles on the street
- [x] Creating vehicles on the street from file data
- [x] Create a map of District 1 (Top-Left: 10.78053, 106.6938; Bottom-Right: 10.7708, 106.7064)
- [x] Create a map of ThuDuc City (Top-Left: 10.87477, 106.7596; Bottom-Right: 10.8582, 106.7767)
- [x] Add some other buildings: Bitexco, Parkson Saigon Tourist Plaza, Saigon Central Post Office, Majestic Hotel, etc
- [x] Create a map below the model of Thu Duc City
- [x] Edit model location on map of Thu Duc City
- [x] Build the Thu Duc City road system

## Street names and coordinates

**ThuDuc City**

<details>
    <summary>Click to expand!</summary>
| Data | Coordinates| The name of the street in the program|
|:----:| :----: | :---- |
|45| 10.8620000000 106.7723770000 |45|
|52| 10.8630000000 106.7697750000 |52|
|56| 10.8660000000 106.7631840000 |56|
|68| 10.8700484369 106.7659628391 |68|
|71| 10.8710335825 106.7642515898 |71|
|72| 10.8623554975 106.7725302279 |72|
|74| 10.8690000000 106.7704090000 |74|

</details>

**District1**

<details>
    <summary>Click to expand!</summary>

| Data | Coordinates| The name of the street in the program|
|:----:| :----: | :---- |
|96| 10.7804410000 106.6986730000 |96|
|97| 10.7790440000 106.6993680000 |97|
|99| 10.7764480000 106.7011760000 |99|
|102| 10.7753510000 106.7067580000 |102|

</details>

_Data are being updated_