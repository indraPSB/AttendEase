window.getCurrentLocation = async () => {
    //console.log("Getting current location...");

    if (navigator.geolocation) {
        try {
            const position = await new Promise((resolve, reject) => {
                navigator.geolocation.getCurrentPosition(resolve, reject);
            });

            //console.log("Current location: ", position.coords.latitude, position.coords.longitude);
            return new Location(position.coords.latitude, position.coords.longitude);
        } catch (error) {
            //console.error("Error getting current location: ", error);
        }
    } else {
        //console.log("Geolocation is not supported by this browser.");
    }

    return new Location(0, 0);
}

class Location {
    constructor(latitude, longitude) {
        this.Latitude = latitude;
        this.Longitude = longitude;
    }
}
