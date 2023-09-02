<template>
    <div class="mx-auto">
        <h3 class="text-center mb-1">Welcome to GameStatsApp</h3>
        <div class="mx-auto" style="max-width:400px;">
            <div class="mb-3">
                <change-username :username="username"></change-username>
            </div>      
            <div class="text-center">
                <span class="lead text-dark">Info:</span>
            </div>            
            <div class="mb-3">
                <ul class="p-0">
                    <li>Add new games using <font-awesome-icon icon="fa-solid fa-plus"/></li>
                    <li>Add/Remove games from lists using <font-awesome-icon icon="fa-solid fa-inbox"/>&nbsp;<font-awesome-icon icon="fa-solid fa-play"/>&nbsp;<font-awesome-icon icon="fa-solid fa-check"/>&nbsp;<font-awesome-icon icon="fa-solid fa-ellipsis"/></li>
                    <li>Import games using <font-awesome-icon icon="fa-solid fa-cloud-arrow-down"/></li>
                    <li>Manage lists and link accounts from Settings <font-awesome-icon icon="fa-solid fa-gear"/></li>
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


