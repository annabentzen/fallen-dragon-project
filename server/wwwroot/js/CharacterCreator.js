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
        face: "faces",
        clothing: "clothes"
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
        if (idx === -1) idx = 0; // fallback
        indexMap[target] = idx;

        // Set the initial image
        $("#" + target).attr("src", `/images/${folderMap[target]}/${imagesMap[target][idx]}`);
        input.val(imagesMap[target][idx]);
    });

    // Function to update image + hidden input
    function updateCharacterPart(target, newIndex) {
        const imageFile = imagesMap[target][newIndex];
        const folder = folderMap[target];

        $("#" + target).attr("src", `/images/${folder}/${imageFile}`);
        $("#" + target + "Input").val(imageFile); // 🔥 update hidden input value
    }

    // Button click handlers
    $(".next").click(function () {
        const target = $(this).data("target");
        const max = imagesMap[target].length;
        indexMap[target] = (indexMap[target] + 1) % max; // wrap around
        updateCharacterPart(target, indexMap[target]);
    });

    $(".prev").click(function () {
        const target = $(this).data("target");
        const max = imagesMap[target].length;
        indexMap[target] = (indexMap[target] - 1 + max) % max; // wrap around backwards
        updateCharacterPart(target, indexMap[target]);
    });
});

