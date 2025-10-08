// Character carousel JS
$(document).ready(function () {
    // Arrays of image filenames
    const hairImages = ["hair1.png", "hair2.png"];
    const faceImages = ["face1.png", "face2.png"];
    const clothingImages = ["clothing1.png", "clothing2.png"];

    // Change the image for the given category
    function changeImage(target, direction) {
        const imagesMap = {
            hair: hairImages,
            face: faceImages,
            clothing: clothingImages
        };

        const img = $("#" + target);            // the <img> element
        const input = $("#" + target + "Input"); // the hidden input to submit form
        let index = imagesMap[target].indexOf(input.val());

        if (direction === 'next') {
            index = (index + 1) % imagesMap[target].length;
        } else {
            index = (index - 1 + imagesMap[target].length) % imagesMap[target].length;
        }

        // Update the image source and hidden input value
        img.attr("src", `/images/${target}/${imagesMap[target][index]}`);
        input.val(imagesMap[target][index]);
    }

    // Button click handlers
    $(".next").click(function () {
        const target = $(this).data("target");
        changeImage(target, "next");
    });

    $(".prev").click(function () {
        const target = $(this).data("target");
        changeImage(target, "prev");
    });
});
