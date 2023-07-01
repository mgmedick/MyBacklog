<template>
    <div>
        <div v-if="!isauth">          
            <span>Not logged in page</span>
        </div>  
        <div v-else>
            <user-gamelists :userid="userid"></user-gamelists>
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






