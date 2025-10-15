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

                // Set initial selection from page data
                if (initialCharacterData.poseId) {
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
