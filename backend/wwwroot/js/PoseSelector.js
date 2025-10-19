
$(document).ready(function () {
    let allPoses = [];

    function fetchPoses() {
        $.ajax({
            url: '/api/poses',
            method: 'GET',
            success: function(data) {
                allPoses = data;
                populateDropdown();

                // Select saved pose if it exists
                if (initialCharacterData.poseId != null) {
                    $('#poseDropdown').val(initialCharacterData.poseId);
                    displayPoseImage(initialCharacterData.poseId);
                }
            }
        });
    }

    function populateDropdown() {
        const poseDropdown = $('#poseDropdown');
        poseDropdown.empty().append('<option value="">Select a pose</option>');
        allPoses.forEach(p => {
            poseDropdown.append($('<option></option>').val(p.id).text(p.name));
        });
    }

    function displayPoseImage(poseId) {
        const poseImageElement = $('#poseImage');
        if (poseId) {
            const selectedPose = allPoses.find(p => p.id == poseId);
            if (selectedPose && selectedPose.imageUrl) {
                poseImageElement.attr('src', '/images/poses/' + selectedPose.imageUrl);
                poseImageElement.show();
            } else {
                poseImageElement.hide();
                poseImageElement.attr('src', '');
            }
        } else {
            poseImageElement.hide();
            poseImageElement.attr('src', '');
        }
    }

    $('#poseDropdown').change(function() {
        displayPoseImage($(this).val());
    });

    fetchPoses();
});
