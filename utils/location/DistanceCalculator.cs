namespace TicDrive.Utils.Location
{
    public static class DistanceCalculator
    {
        //haversine formula
        public static double CalculateDistanceInMeters(double lat1, double lon1, double lat2, double lon2)
        {
            var R = 6371000;
            var lat1Rad = Math.PI * lat1 / 180.0;
            var lat2Rad = Math.PI * lat2 / 180.0;
            var deltaLat = Math.PI * (lat2 - lat1) / 180.0;
            var deltaLon = Math.PI * (lon2 - lon1) / 180.0;

            var a = Math.Sin(deltaLat / 2) * Math.Sin(deltaLat / 2) +
                    Math.Cos(lat1Rad) * Math.Cos(lat2Rad) *
                    Math.Sin(deltaLon / 2) * Math.Sin(deltaLon / 2);

            var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));

            return R * c; //meters
        }
    }
}
