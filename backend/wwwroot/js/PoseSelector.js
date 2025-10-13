$(document).ready(function () {
            // Function to fetch poses from your API
            function fetchPoses() {
                $.ajax({
                    url: '/api/poses', // Your API endpoint for poses
                    method: 'GET',
                    success: function (data) {
                        const poseDropdown = $('#poseDropdown');
                        data.forEach(function (pose) {
                            poseDropdown.append($('<option></option>').val(pose.imageUrl).text(pose.name));
                        });
                    },
                    error: function (error) {
                        console.error("Error fetching poses:", error);
                    }
                });
            }

            // Call the function to populate the dropdown when the page loads
            fetchPoses();

            // Event listener for dropdown change
            $('#poseDropdown').change(function () {
                const selectedPoseImageUrl = $(this).val();
                const poseImageElement = $('#poseImage');

                if (selectedPoseImageUrl) {
                    poseImageElement.attr('src', '/images/poses/' + selectedPoseImageUrl);
                    poseImageElement.show(); // Make sure the image is visible
                } else {
                    poseImageElement.hide(); // Hide if no pose is selected
                    poseImageElement.attr('src', ''); // Clear the src
                }
            });
        });