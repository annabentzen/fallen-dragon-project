$(document).ready(function() {
    // Fetch poses from the server
    $.get('/api/characterposes', function(poses) {
        const poseSelector = $('#poseSelector');
        poses.forEach(pose => {
            const option = $('<option></option>')
                .attr('value', pose.id)
                .text(pose.name);
            poseSelector.append(option);
        });
    });

    // Handle pose selection change
    $('#poseSelector').change(function() {
        const selectedPoseId = $(this).val();
        // You can add code here to update the character's pose in the UI
        console.log('Selected Pose ID:', selectedPoseId);
    });
});