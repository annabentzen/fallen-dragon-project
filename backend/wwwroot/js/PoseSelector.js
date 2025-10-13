
$(document).ready(function () {
    let allPoses = []; 

    function fetchPoses() {
        $.ajax({
            url: '/api/poses', 
            method: 'GET',
            success: function (data) {
                allPoses = data; 
                const poseDropdown = $('#poseDropdown');
                poseDropdown.empty().append('<option value="">-- Select a Pose --</option>'); 
                
                data.forEach(function (pose) {
                    poseDropdown.append($('<option></option>').val(pose.id).text(pose.name));
                });

                // *** Use the global initialCharacterData to set the initial selection ***
                if (initialCharacterData.poseId !== "null") {
                    poseDropdown.val(initialCharacterData.poseId);
                    displayPoseImage(initialCharacterData.poseId); 
                }
            },
            error: function (error) {
                console.error("Error fetching poses:", error);
            }
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
                console.warn("Pose image URL not found for ID:", poseId);
                poseImageElement.hide();
                poseImageElement.attr('src', '');
            }
        } else {
            poseImageElement.hide();
            poseImageElement.attr('src', '');
        }
    }

    fetchPoses();

    $('#poseDropdown').change(function () {
        const selectedPoseId = $(this).val(); 
        displayPoseImage(selectedPoseId);
    });
});