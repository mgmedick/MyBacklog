<template>
    <div class="mx-auto">
        <h3 class="text-center mb-1">Welcome to mybacklog.io</h3>
        <div class="mx-auto" style="max-width:400px;"> 
            <div v-if="isdemo" class="mx-auto mb-3">
                <div class="card text-white bg-warning">
                    <div class="card-header">Warning</div>
                    <div class="card-body">
                        <p class="card-text">You are in <span class="fw-bold">demo</span> mode, any user data will not be preserved outside this session. If you like what you see sign-up to create a permanent user.</p>
                    </div>
                </div>  
            </div>        
            <div v-else class="mb-3">
                <change-username :username="username"></change-username>
            </div>                  
            <div class="text-center">
                <span class="lead text-dark">Instructions:</span>
            </div>            
            <div class="mb-3">
                <ul class="list-group list-group-flush">
                    <li class="list-group-item">Add new games to your lists &nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-plus"/></li>
                    <li class="list-group-item">Move games between lists &nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-inbox"/>&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-play"/>&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-check"/>&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-ellipsis"/></li>
                    <li class="list-group-item">Import games &nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-cloud-arrow-down"/></li>
                    <li class="list-group-item">Manage lists from Settings&nbsp;&nbsp;<font-awesome-icon icon="fa-solid fa-gear"/></li>
                </ul>                
            </div>            
        </div>
    </div>
</template>
<script>    
    export default {
        name: "Welcome",
        props: {
            username: String,
            isdemo: Boolean
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


