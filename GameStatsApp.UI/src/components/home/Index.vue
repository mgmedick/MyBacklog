<template>
    <div>
        <div v-if="!isauth">          
            <span>Not logged in page</span>
        </div>  
        <div v-else>
            <user-game-lists :userid="userid"></user-game-lists>
        </div>
    </div>    
</template>
<script>
    export default {
        name: "Index",
        props: {
            isauth: Boolean,
            userid: String
        },
        data: function () {
            return {
            };
        },
        watch: {},
        created: function () {
        },
        methods: {},
    };
</script>






