var LastFmKey = "4a06f33fa225792783b9c7c2e6b9fde6";

//function getAlbumImg(title, artist) {
//    var url = encodeURI("https://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key="
//        + LastFmKey + "&artist=" + artist + "&album=" + title + "&autocorrect=1&format=json");

//    $.getJSON(
//        url, function (json) { 
//            var src = json.album.image[4]["#text"];
//            $("#img").html("<img id='album-art' alt = " + alt + " src = " + src + ">");  
//        });
//}

function getAlbumInfo(title, artist) {
    var url = encodeURI("https://ws.audioscrobbler.com/2.0/?method=album.getinfo&api_key="
        + LastFmKey + "&artist=" + artist + "&album=" + title + "&autocorrect=1&format=json");

    $.getJSON(
        url, function (json) {
            var alt = json.album.name.split(" ").join("") + "AlbumArt";
            var src = json.album.image[4]["#text"];
            var util = { sum: 0 };

            var summary = json.album.wiki != null ? json.album.wiki.summary : "The Synopsis for this Album is Unavailable on Last.Fm";
            $("#img").html("<img id='album-art' alt = " + alt + " src = " + src + ">");
            $("#bio").html(summary);

            json.album.tracks.track.forEach(function (song) {
                this.sum += Number(song.duration);
                $("#tracklist").append("<tr><td>" + song.name + "</td><td>" + secondsToTime(song.duration) + "</td></tr>");
            }, util);


            $("#totalDuration").append("<tr class=table-secondary><td><b>Total Duration<b></td>" +
                "<td>" +
                "<b>" +
                secondsToTime(util.sum) +
                "</b>" +
                "</td>" +
                "</tr>");
        });
}