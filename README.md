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
- [x] Add some other buildings: Bitexco, Parkson Saigon Tourist Plaza, Saigon Central Post Office, Majestic Hotel, etc
- [x] Create a map below the model of Thu Duc City
- [ ] Edit model location on map of Thu Duc City (@Mohit please help me fix it)
- [x] Build the Thu Duc City road system

## Street names and coordinates

| Data | Coordinates| The name of the street in the program|
|:----:| :----: | :---- |
|45| 10.8620000000 106.7723770000 |45.Duong14|
|52| 10.8630000000 106.7697750000 |52.GanDuong13|
|56| 10.8660000000 106.7631840000 |56.DauDuong13|
|68| 10.8700484369 106.7659628391 |68.DuongE|
|71| 10.8710335825 106.7642515898 |71.PhamVanDong|
|72| 10.8623554975 106.7725302279 |72.14Giao13|
|74| 10.8690000000 106.7704090000 |74.DuongSo2|

_The remaining data has no coordinates to deploy_