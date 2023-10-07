<template>
    <div class="mx-auto">
        <h3 class="text-center mb-1">Welcome to mybacklog.io</h3>
        <div class="mx-auto" style="max-width:400px;">
            <div v-if="isdemo">
                <div class="mx-auto text-center mb-3">
                    <div class="d-flex">
                        <font-awesome-icon icon="fa-solid fa-triangle-exclamation" size="lg" class="text-warning me-2"/>
                        <div class="text-dark"><span>You are in Demo mode so your changes won't be retained beyond this session</span></div>
                    </div>
                </div>
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


