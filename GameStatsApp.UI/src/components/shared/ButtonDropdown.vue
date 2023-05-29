<template>
    <div class="btn-group" :class="{ show : state }" @click="toggleDropdown">
        <button class="btn dropdown-toggle" :class="btnclasses" type="button" data-bs-toggle="dropdown" aria-expanded="false">
            <slot name="text"></slot>
        </button>
        <div class="dropdown-menu" :class="[listclasses]" aria-labelledby="dropdownMenuButton" :style="[ state ? { display:'block' } : { display:'none' } ] " >
            <slot name="options"></slot>
        </div>
    </div>
</template>
<script>
    export default {
        name: "ButtonDropdown",
        props: {
            btnclasses: String,
            listclasses: String
        },
        data() {
            return {
                state: false
            }
        },
        // computed: {
        //     popperPlacement() {
        //         var result = '';

        //         if (this.listclasses.indexOf("dropdown-menu-end") > -1)
        //         {
        //             result = 'bottom-end'
        //         }

        //         return result;
        //     }            
        // },           
        methods: {
            toggleDropdown(e) {
                this.state = !this.state
            },
            close(e) {
                if (!this.$el.contains(e.target)) {
                    this.state = false;
                }
            }
        },
        mounted() {
            document.addEventListener('click', this.close)
        },
        beforeDestroy() {
            document.removeEventListener('click', this.close)
        }
    };
</script>






