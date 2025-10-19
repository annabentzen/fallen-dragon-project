$(document).ready(function () {
    const poseDropdown = $("#poseDropdown");
    const poseImage = $("#poseImage");

    poseDropdown.on("change", function () {
        const poseId = $(this).val();
        if (poseId) {
            const poseOption = $(this).find("option:selected").text().trim().toLowerCase();
            const poseFileName = poseOption.replace(/\s+/g, "_") + ".png";
            poseImage.attr("src", "/images/poses/" + poseFileName);
        } else {
            poseImage.attr("src", "/images/base.png");
        }
    });
});
