<template>
    <div class="mx-auto">
        <h3 class="text-center mb-1">Welcome to MyBacklog</h3>
        <div class="mx-auto" style="max-width:400px;">
            <div class="mb-3">
                <change-username :username="username"></change-username>
            </div>      
            <div class="text-center">
                <span class="lead text-dark">Instructions:</span>
            </div>            
            <div class="mb-3">
                <ul class="list-group list-group-flush">
                    <li class="list-group-item">Add new games using&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-plus"/></li>
                    <li class="list-group-item">Add/Remove games using&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-inbox"/>&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-play"/>&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-check"/>&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-ellipsis"/></li>
                    <li class="list-group-item">Import games using&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-cloud-arrow-down"/></li>
                    <li class="list-group-item">Manage lists from User Settings&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-gear"/></li>
                </ul>                
            </div>            
        </div>
    </div>
</template>
<script>    
    export default {
        name: "Welcome",
        props: {
            username: String
        },
        data() {
            return {
            }
        },
        computed: {
        },
        mounted: function () {
        },
        methods: {                      
        }
    };
</script>


