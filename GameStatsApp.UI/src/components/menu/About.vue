<template>
    <div class="mx-auto">
        <h2 class="text-center mb-3">About</h2>
        <div class="mx-auto" style="max-width:400px;">   
            <p class="lead text-dark">A site where you can import, add and remove games from your backlog or your own user created lists.</p>
            <div class="text-dark mb-3"><span>Data is provided by <a href="https://www.igdb.com/" class="link-underline-info fw-bold">igdb.com</a>.</span></div>
            <div v-if="aboutvm.userCount > 15" class="text-dark"><span><strong>{{ aboutvm.userCount }}</strong> people like you already use this site. A few minutes ago, someone added <strong>{{ aboutvm.gameName }}</strong> to their <strong>{{ aboutvm.userListName }}</strong> list.</span></div>
        </div>
    </div>
</template>
<script>    
    export default {
        name: "About",
        props: {
            aboutvm : Object
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


