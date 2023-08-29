<template>
    <div class="mx-auto">
        <h2 class="text-center mb-1">User Settings</h2>
        <div class="mx-auto" style="max-width:400px;">
            <div class="mb-4">
                <change-username :username="usersettingsvm.username"></change-username>
            </div>
            <div class="text-center">
                <h4>Manage Lists</h4>
            </div>   
            <manage-user-lists></manage-user-lists>
        </div>                             
    </div>
</template>
<script>    
    export default {
        name: "UserSettings",
        props: {
            usersettingsvm: Object
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


