$(document).ready(function () {
    // Image arrays
    const imagesMap = {
        hair: ["hair1.png", "hair2.png", "hair3.png"],
        face: ["face1.png", "face2.png", "face3.png"],
        clothing: ["clothing1.png", "clothing2.png", "clothing3.png"]
    };

    // Folder map to match actual folders
    const folderMap = {
        hair: "hair",
        face: "faces",      // matches your ~/images/faces/
        clothing: "clothes" // matches your ~/images/clothes/
    };

    // Initialize index for each category
    const indexMap = {
        hair: 0,
        face: 0,
        clothing: 0
    };

    // Set initial index based on hidden input values
    Object.keys(indexMap).forEach(function (target) {
        const input = $("#" + target + "Input");
        const currentValue = input.val();
        let idx = imagesMap[target].indexOf(currentValue);
        if (idx === -1) idx = 0; // fallback to first image if not found
        indexMap[target] = idx;

        // Set the initial image
        $("#" + target).attr("src", `/images/${folderMap[target]}/${imagesMap[target][idx]}`);
        input.val(imagesMap[target][idx]);
    });

    // Handle next/prev buttons
    $("button.next, button.prev").click(function () {
        const target = $(this).data("target");
        const maxIndex = imagesMap[target].length - 1;

        if ($(this).hasClass("next")) {
            indexMap[target] = (indexMap[target] + 1) > maxIndex ? 0 : indexMap[target] + 1;
        } else {
            indexMap[target] = (indexMap[target] - 1) < 0 ? maxIndex : indexMap[target] - 1;
        }

        // Update image and hidden input
        $("#" + target).attr("src", `/images/${folderMap[target]}/${imagesMap[target][indexMap[target]]}`);
        $("#" + target + "Input").val(imagesMap[target][indexMap[target]]);
    });
});

