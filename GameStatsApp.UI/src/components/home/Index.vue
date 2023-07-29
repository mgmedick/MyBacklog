<template>
    <div>
        <div v-if="!indexvm.isAuth">          
            <span>Not logged in page</span>
        </div>  
        <div v-else>
            <user-gamelists :indexvm="indexvm"></user-gamelists>
        </div>
    </div>    
</template>
<script>
    export default {
        name: "Index",
        props: {
            indexvm: Object
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






